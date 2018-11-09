using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;
using Mouledoux.Components;
using UnityEngine;
using UnityEngine.AI;

public class TeleportAI : MonoBehaviour
{
    private Mediator.Subscriptions _subscriptions;
    public Transform TeleportHere;
    private Callback _teleportCallback;

	// Use this for initialization
	void Awake () {
		_subscriptions = new Mediator.Subscriptions();
	    _teleportCallback += Teleport;
        _subscriptions.Subscribe("Teleport", _teleportCallback);
	}

    private void Teleport(Packet emptyPacket)
    {
        StartCoroutine(StartTeleport());
    }

    private IEnumerator StartTeleport()
    {
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForEndOfFrame();
        gameObject.GetComponent<AIGuideBehaviour>().GuideStartPos = TeleportHere.position;
        gameObject.transform.parent = TeleportHere;
        gameObject.transform.position = TeleportHere.position;
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
    }

    public void OnDestroy()
    {
        _subscriptions.UnsubscribeAll();
    }
}
