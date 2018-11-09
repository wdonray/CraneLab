using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mouledoux.Callback;
using Mouledoux.Components;
using UnityEngine.Events;
using UnityEngine.UI;

public class GuideHelper : MonoBehaviour
{
    public static int Index;
    public Text CurrentTaskText;
    [HideInInspector] public List<AIGuideBehaviour> Riggers = new List<AIGuideBehaviour>();
    public List<GameObject> Loads = new List<GameObject>();
    public List<GameObject> Zones = new List<GameObject>();
    public Dictionary<int, LoadAndZone> LoadToZone = new Dictionary<int, LoadAndZone>();

    private Mediator.Subscriptions _subscriptions;
    private Callback _taskCallback, _emergancyCallback;

    public bool reached, tearEnabled;
    // Use this for initialization
    void Awake()
    {
        _subscriptions = new Mediator.Subscriptions();
        _taskCallback += UpdateTaskText;
        Index = 0;
        foreach (var rigger in FindObjectsOfType<AIGuideBehaviour>())
        {
            Riggers.Add(rigger);
            _subscriptions.Subscribe(rigger.gameObject.GetInstanceID().ToString(), _taskCallback);
        }

        for (int i = 0; i < Loads.Count; i++)
        {
            LoadToZone.Add(i, new LoadAndZone(Loads[i], Zones[i]));
        }

        _emergancyCallback += UpdateEmergancyText;
        _subscriptions.Subscribe("EmergancyCallback", _emergancyCallback);
    }

    // Update is called once per frame
    void Update()
    {
        ActiveLoadToZone(Index);
    }

    void ActiveLoadToZone(int index)
    {
        if (index < Loads.Count)
        {
            foreach (var rigger in Riggers)
            {
                rigger.SetDropZone(LoadToZone[index].Zone.transform);
                rigger.SetLoad(LoadToZone[index].Load.transform);
            }
        }
        else
        {
            foreach (var rigger in Riggers)
            {
                rigger._complete = true;
                if (rigger.m_tieOnly == false)
                {
                    SendToAnimator.stop = false;
                    SendToAnimator.ResetAllTriggers(rigger.gameObject);
                    SendToAnimator.SendTrigger(rigger.gameObject, "JobComplete");
                }
                else
                {
                    rigger.Agent.isStopped = true;
                    SendToAnimator.SendTrigger(rigger.gameObject, "Idle");
                }
            }
        }
    }

    private void UpdateTaskText(Packet emptyPacket)
    {
        CurrentTaskText.gameObject.SetActive(true);

        StartCoroutine(ChangeText());
        StartCoroutine(WaitAxisCheck());
    }


    private IEnumerator ChangeText()
    {
        if (CurrentTaskText.gameObject.activeInHierarchy)
        {
            if (Index < Loads.Count)
            {
                if (reached)
                {
                    CurrentTaskText.text = "Good Job";
                    yield return new WaitForSeconds(2);
                    CurrentTaskText.text = "Move " + LoadToZone[Index].Load.transform.parent.name + " to " +
                                           LoadToZone[Index].Zone.transform.tag;
                    reached = false;
                }
                else
                {
                    CurrentTaskText.text = "Move " + LoadToZone[Index].Load.transform.parent.name + " to " +
                                           LoadToZone[Index].Zone.transform.tag;
                }
            }
            else
            {
                CurrentTaskText.text = "Job Complete";
            }
        }
    }
    private IEnumerator WaitAxisCheck()
    {
        var newAxisCheck = new AxisCheckStruct("RIGHT_VERTICAL", new UnityEvent(), StartTaskComplete, 0, 0.2f);
        yield return new WaitUntil(() => Input.GetAxis(newAxisCheck.AxisString) >= newAxisCheck.MaxValue);
        yield return new WaitUntil(() => Input.GetAxis(newAxisCheck.AxisString) <= newAxisCheck.MinValue);
        newAxisCheck.OnComplete.Invoke();
    }

    private IEnumerator TaskOff()
    {
        yield return new WaitForSeconds(2);
        CurrentTaskText.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private void StartTaskComplete()
    {
        StartCoroutine(TaskOff());
    }

    public void OnDestroy()
    {
        _subscriptions.UnsubscribeAll();
    }

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

    public struct LoadAndZone
    {
        public GameObject Load, Zone;

        public LoadAndZone(GameObject load, GameObject zone) : this()
        {
            Load = load;
            Zone = zone;
        }
    }

    public void UpdateEmergancyText(Packet emptyPacket)
    {
        CurrentTaskText.gameObject.SetActive(true);
        if (tearEnabled)
        {
            if (LoadToZone[Index].Load.transform.parent.GetComponentInChildren<TearTest>()._passed)
            {
                CurrentTaskText.text = "You Passed!";
            }
            else if (LoadToZone[Index].Load.transform.parent.GetComponentInChildren<TearTest>()._failed)
            {
                CurrentTaskText.text = "You Failed!";
            }
        }
    }
}
