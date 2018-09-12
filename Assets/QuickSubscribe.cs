using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSubscribe : MonoBehaviour
{
    private Mouledoux.Components.Mediator.Subscriptions m_subscriptions = new Mouledoux.Components.Mediator.Subscriptions();
    private Mouledoux.Callback.Callback m_subCallback;

    public string m_subMessage;
    public UnityEngine.Events.UnityEvent m_event;
       
	void Start ()
    {
        m_subCallback = InvokeUnityEvent;
        m_subscriptions.Subscribe(m_subMessage, m_subCallback);
	}

    private void InvokeUnityEvent(Mouledoux.Callback.Packet emptyPacket)
    {
        m_event.Invoke();
    }
}