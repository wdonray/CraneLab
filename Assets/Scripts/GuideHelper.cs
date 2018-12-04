using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mouledoux.Callback;
using Mouledoux.Components;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GuideHelper : MonoBehaviour
{
    public TestType TestType;
    public GameObject CompleteParticleSystem;
    public static int Index, RandomIndexLoad;
    public Text CurrentTaskText;
    [HideInInspector] public List<AIGuideBehaviour> Riggers = new List<AIGuideBehaviour>();
    public List<GameObject> Loads = new List<GameObject>();
    public List<GameObject> Zones = new List<GameObject>();

    private Mediator.Subscriptions _subscriptions;
    [HideInInspector] public Callback _taskCallback, _emergancyCallback;

    public bool reached;

    [SerializeField] private string PassedMessage, FailedMessage;
    // Use this for initialization
    void Awake()
    {
        CompleteParticleSystem.gameObject.SetActive(false);
        _subscriptions = new Mediator.Subscriptions();
        _taskCallback += UpdateTaskText;
        Index = 0; RandomIndexLoad = 0;

        foreach (var rigger in FindObjectsOfType<AIGuideBehaviour>())
        {
            Riggers.Add(rigger);
            _subscriptions.Subscribe(rigger.gameObject.GetInstanceID().ToString(), _taskCallback);
        }

        foreach (var rigger in Riggers)
        {
            rigger.TestType = TestType;
        }

        switch (TestType)
        {
            case TestType.Personnel:
                _subscriptions.Subscribe(FindObjectOfType<AIGrabLift>().gameObject.GetInstanceID().ToString(), _taskCallback);
                break;
            case TestType.Infinite:
                Index = Random.Range(0, Zones.Count);
                RandomIndexLoad = Random.Range(0, Loads.Count);
                break;
        }

        _emergancyCallback += UpdateEmergancyText;
        _subscriptions.Subscribe("EmergancyCallback", _emergancyCallback);
    }

    // Update is called once per frame
    void Update()
    {
        ActiveLoadToZone();
    }

    /// <summary>
    ///     Each riggers zone and load will be set unless the game is over and complete 
    /// </summary>
    void ActiveLoadToZone()
    {
        switch (TestType)
        {
            case TestType.Personnel:
            {
                switch (Index)
                {
                    case 0:
                    {
                        foreach (var rigger in Riggers)
                        {
                            rigger.SetDropZone(Zones[0].transform);
                            rigger.SetLoad(Loads[0].transform);
                        }
                        break;
                    }
                    case 1:
                    {
                        foreach (var rigger in Riggers)
                        {
                            rigger.SetDropZone(Zones[1].transform);
                            rigger.SetLoad(Loads[0].transform);
                        }
                        break;
                    }

                    default:
                    {
                        Completed();
                    }
                        break;
                }
                break;
            }
            case TestType.Infinite:
            {
                foreach (var rigger in Riggers)
                {
                    rigger.SetDropZone(Zones[Index].transform);
                    rigger.SetLoad(Loads[RandomIndexLoad].transform);
                }

                break;
            }
            default:
            {
                if (Index < Loads.Count)
                {
                    foreach (var rigger in Riggers)
                    {
                        rigger.SetDropZone(Zones[Index].transform);
                        rigger.SetLoad(Loads[Index].transform);
                    }
                }
                else
                {
                    Completed();
                }

                break;
            }
        }
    }

    private void Completed()
    {
        foreach (var rigger in Riggers)
        {
            rigger._complete = true;
            if (rigger.m_tieOnly == false)
            {
                if (rigger.m_dead == false)
                {
                    SendToAnimator.stop = false;
                    SendToAnimator.ResetAllTriggers(rigger.gameObject);
                    SendToAnimator.SendTrigger(rigger.gameObject, "JobComplete");
                }
            }
            else
            {
                if (rigger.m_dead == false)
                {
                    rigger.Agent.isStopped = true;
                    SendToAnimator.SendTrigger(rigger.gameObject, "Idle");
                }
            }
        }
    }

    /// <summary>
    ///     Start the change text event and wait for movement
    /// </summary>
    /// <param name="emptyPacket"></param>
    private void UpdateTaskText(Packet emptyPacket)
    {
        CurrentTaskText.gameObject.SetActive(true);

        StartCoroutine(ChangeText());
        StartCoroutine(WaitAxisCheck());
    }

    /// <summary>
    ///     Update the text based on the current game state
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeText()
    {
        if (CurrentTaskText.gameObject.activeInHierarchy)
        {
            if (TestType == TestType.Infinite)
            {
                if (reached)
                {
                    CurrentTaskText.text = "Good Job";
                    yield return new WaitForSeconds(3);
                    CurrentTaskText.text = "Move " + Loads[RandomIndexLoad].transform.parent.name + " to " +
                                           Zones[Index].transform.tag;
                    reached = false;
                }
                else
                {
                    CurrentTaskText.text = "Move " + Loads[RandomIndexLoad].transform.parent.name + " to " +
                                           Zones[Index].transform.tag;
                }

            }
            else
            {
                var sum = Index - 1;
                if (Index < Loads.Count)
                {
                    if (reached)
                    {
                        CurrentTaskText.text = "Good Job";
                        var particle = Instantiate(CompleteParticleSystem,
                            Loads[sum].transform.parent.GetChild(2).transform);
                        particle.transform.localPosition = new Vector3(particle.transform.localPosition.x,
                            particle.transform.localPosition.y + 1);
                        particle.gameObject.SetActive(true);
                        yield return new WaitForSeconds(3);
                        Destroy(particle);
                        CurrentTaskText.text = "Move " + Loads[Index].transform.parent.name + " to " +
                                               Zones[Index].transform.tag;
                        reached = false;
                    }
                    else
                    {
                        CurrentTaskText.text = "Move " + Loads[Index].transform.parent.name + " to " +
                                               Zones[Index].transform.tag;
                    }
                }
                else
                {
                    CurrentTaskText.text = "Job Complete";
                    var particle = Instantiate(CompleteParticleSystem,
                        Loads[sum].transform.parent.GetChild(2).transform);
                    particle.transform.localPosition = new Vector3(particle.transform.localPosition.x,
                        particle.transform.localPosition.x + 1);
                    particle.gameObject.SetActive(true);
                    yield return new WaitForSeconds(3);
                    Destroy(particle);
                }
            }
        }
    }

    /// <summary>
    ///     Wait for input from users
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitAxisCheck()
    {
        var newAxisCheck = new AxisCheckStruct("RIGHT_VERTICAL", new UnityEvent(), StartTaskComplete, 0, 0.2f);
        yield return new WaitUntil(() => Input.GetAxis(newAxisCheck.AxisString) >= newAxisCheck.MaxValue);
        yield return new WaitUntil(() => Input.GetAxis(newAxisCheck.AxisString) <= newAxisCheck.MinValue);
        newAxisCheck.OnComplete.Invoke();
    }

    /// <summary>
    ///     Turn text off and stop all coroutines
    /// </summary>
    /// <returns></returns>
    private IEnumerator TaskOff()
    {
        yield return new WaitForSeconds(3);
        CurrentTaskText.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    /// <summary>
    ///     Start the task of coroutine
    /// </summary>
    private void StartTaskComplete()
    {
        StartCoroutine(TaskOff());
    }

    public void OnDestroy()
    {
        _subscriptions.UnsubscribeAll();
    }

    /// <summary>
    ///     Used to store and check axis information
    /// </summary>
    public struct AxisCheckStruct
    {
        public string AxisString;
        public float MinValue;
        public float MaxValue;
        public UnityEvent OnComplete;
        public Action action;

        public AxisCheckStruct(string axisString, UnityEvent onComplete, Action mAction, float minValue, float maxValue)
        {
            AxisString = axisString;
            OnComplete = onComplete;
            action = mAction;
            MinValue = minValue;
            MaxValue = maxValue;

            onComplete.AddListener(action.Invoke);
        }
    }

    /// <summary>
    ///     If you passed or failed the tear test update this text
    /// </summary>
    /// <param name="emptyPacket"></param>
    public void UpdateEmergancyText(Packet emptyPacket)
    {
        CurrentTaskText.gameObject.SetActive(true);
        if (TestType == TestType.Break)
        {
            if (Loads[Index].transform.parent.GetComponentInChildren<TearTest>()._passed)
            {
                CurrentTaskText.text = "You Passed!";
                var particle = Instantiate(CompleteParticleSystem, Loads[Index].transform.parent.GetChild(2).transform);
                particle.gameObject.SetActive(true);
                Mediator.instance.NotifySubscribers(PassedMessage, new Packet());
                StartCoroutine(DestroyAfter(3, particle));
            }
            else if (Loads[Index].transform.parent.GetComponentInChildren<TearTest>()._failed)
            {
                CurrentTaskText.text = "You Failed!";
                Mediator.instance.NotifySubscribers(FailedMessage, new Packet());
            }
        }
        else
        {
            CurrentTaskText.text = "You Failed!";
        }
    }

    private IEnumerator DestroyAfter(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
