using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [HideInInspector] public static int Index, RandomIndexLoad;
    public Text CurrentTaskText;
    [HideInInspector] public List<AIGuideBehaviour> Riggers = new List<AIGuideBehaviour>();
    public List<GameObject> Loads = new List<GameObject>();
    public List<GameObject> Zones = new List<GameObject>();

    private Mediator.Subscriptions _subscriptions;
    [HideInInspector] public Callback _taskCallback, _emergancyCallback, _teleportCallback;

    [HideInInspector] public bool reached;

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
                FindObjectOfType<ActiveCycler>().ActivateNext();
                break;
            case TestType.Infinite:
                {
                    FindObjectOfType<ActiveCycler>().m_toggleObjects.Add(GameObject.FindGameObjectWithTag("GuideCamera"));
                    GameObject.FindGameObjectWithTag("GuideCamera").GetComponent<MeshRenderer>().enabled = true;
                    Index = Random.Range(0, Zones.Count);
                    RandomIndexLoad = Random.Range(0, Loads.Count);

                    foreach (var load in Loads)
                    {
                        var randomSelection = Random.Range(0, Zones.Count);
                        var pos = load.transform.parent.position;

                        load.transform.parent.position = Zones[randomSelection].transform.position;
                        Zones[randomSelection].transform.position = pos;

                        if (load != Loads[RandomIndexLoad])
                        {
                            load.gameObject.SetActive(false);
                            load.GetComponent<LinkPullTowards>().enabled = false;
                        }
                    }
                    break;
                }

            default:
                break;
        }

        _emergancyCallback += UpdateEmergancyText;
        _subscriptions.Subscribe("EmergancyCallback", _emergancyCallback);
        _teleportCallback += TeleportAi;
        _subscriptions.Subscribe("InfiniteTeleport", _teleportCallback);
    }



    // Update is called once per frame
    void Update()
    {
        ActiveLoadToZone();

        for (var i = 0; i < Riggers.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(Riggers[i].transform.position + Vector3.up, Vector3.up * 999, out hit, 100))
            {
                if (hit.transform.CompareTag("Hook") || hit.transform.CompareTag("DropZone") || hit.transform.CompareTag("Link"))
                {
                    Riggers[i].AboveHead = true;
                    Mediator.instance.NotifySubscribers("WarnUser", new Packet());
                }
                else
                {
                    Riggers[i].AboveHead = false;
                    Mediator.instance.NotifySubscribers("DoNotWarnUser", new Packet());
                }
            }
            else
            {
                var index = 0;

                if (i == Riggers.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index += 1;
                }

                if (!Riggers[index].AboveHead)
                {
                    Riggers[i].AboveHead = false;
                    Mediator.instance.NotifySubscribers("DoNotWarnUser", new Packet());
                }
            }
        }
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

    private bool _sentPassedMessage;

    private void Completed()
    {
        if (_sentPassedMessage == false)
        {
            Mediator.instance.NotifySubscribers(PassedMessage, new Packet());
            _sentPassedMessage = true;
        }

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
                                           Zones[Index].transform.name;
                    reached = false;
                }
                else
                {
                    CurrentTaskText.text = "Move " + Loads[RandomIndexLoad].transform.parent.name + " to " +
                                           Zones[Index].transform.name;
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
        if (TestType != TestType.Infinite)
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
            Mediator.instance.NotifySubscribers(FailedMessage, new Packet());
        }
    }

    private IEnumerator DestroyAfter(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }

    private void TeleportAi(Packet emptyPacket)
    {
        var sortedList = Riggers.OrderBy(x => x.m_tieOnly).ToList();
        for (var i = 0; i < sortedList.Count; i++)
        {
            if (!sortedList[i].m_tieOnly)
            {
                //rigger.transform.position = Zones[Index].transform.GetChild(4).GetChild(0).transform.position;
                sortedList[i].transform.position = sortedList[i + 1].transform.position;
                sortedList[i].StoreHookPos = sortedList[i].HookPos;
                sortedList[i].StartCheckHoist();
            }
            else
            {
                sortedList[i].GuideStartPos = Zones[Index].transform.GetChild(4).GetChild(1).transform.position;
                sortedList[i].transform.position = Zones[Index].transform.GetChild(4).GetChild(1).transform.position;
            }

            StartCoroutine(RotateTowardsCrane(sortedList[i].transform, sortedList[i].CranePos, 10));
        }
    }


    private IEnumerator RotateTowardsCrane(Transform ai, Vector3 cranePos, float angle)
    {
        while (Vector3.Angle(ai.transform.forward, cranePos - ai.transform.position) > angle)
        {
            ai.LookAt(cranePos);
        }

        yield return null;
    }
}
