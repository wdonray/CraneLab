using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;
using Mouledoux.Components;
using UnityEngine;

public class PersonalLiftTest : MonoBehaviour
{
    public float SpeedLimit;
    private Rigidbody _rigidbody => GetComponent<Rigidbody>();
    private List<bool> _failChecks;
    private Mediator.Subscriptions _subscriptions;
    private Callback _failedPersonalLift;

    void Awake ()
	{
        _subscriptions= new Mediator.Subscriptions();

        _failChecks = new List<bool>
        {
            MovedTooFast()
        };

        _subscriptions.Subscribe("FailedPersonalLift", _failedPersonalLift);
	}
	
	IEnumerator Start () {
		yield return new WaitUntil(CheckForAnyFails);
        Mediator.instance.NotifySubscribers("FailedPersonalLift", new Packet());
	}

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool MovedTooFast()
    {
        return _rigidbody.velocity.magnitude >= SpeedLimit;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool CheckForAnyFails()
    {
        foreach (var check in _failChecks)
        {
            if (check)
            {
                return true;
            }
        }
        return false;
    }
}
