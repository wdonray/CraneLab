using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Mouledoux.Callback;
using Mouledoux.Components;
using UnityEngine;

[RequireComponent(typeof(LoadMetrics))]
[RequireComponent(typeof(QuickSubscribe))]
public class PersonalLiftTest : MonoBehaviour
{
    [SerializeField] private string FailedMessage;
    private LoadMetrics _loadMetrics => GetComponent<LoadMetrics>();
    private Mediator.Subscriptions _subscriptions;
    private readonly Callback _failedPersonalLift;
    private QuickSubscribe _quickSubscribe => GetComponent<QuickSubscribe>();

    private AIGrabLift aibGrabLift;

    void Awake()
    {
        aibGrabLift = FindObjectOfType<AIGrabLift>();
        _quickSubscribe.m_subMessage = FailedMessage;
        _subscriptions = new Mediator.Subscriptions();
        _subscriptions.Subscribe(FailedMessage, _failedPersonalLift);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(CheckForAnyFails);
        if (!aibGrabLift._completed)
            Mediator.instance.NotifySubscribers(FailedMessage, new Packet());
    }

    /// <summary>
    ///     Checks if any bool in the list is true
    /// </summary>
    /// <returns></returns>
    private bool CheckForAnyFails()
    {
        if (_loadMetrics == null) return false;
        if (_loadMetrics.RanIntoSomething == false)
        {
            foreach (var check in _loadMetrics.FailChecks)
            {
                if (check())
                {
                    print(check.Method.Name + "(): Failed this check");
                    return true;
                }
            }
        }
        else
        {
            print("RanIntoSomething: Failed this check");
            return true;
        }

        return false;
    }

    public void OnDestroy()
    {
        _subscriptions.UnsubscribeAll();
    }
}
