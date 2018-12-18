using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTTimeScore : MonoBehaviour
{
    private Mouledoux.Components.Mediator.Subscriptions m_subscriptions = new Mouledoux.Components.Mediator.Subscriptions();

	private void Start ()
    {
        Mouledoux.Callback.Callback updateTime = null;
        updateTime += AddTimeToToal;
        m_subscriptions.Subscribe("update_total_time", updateTime);

	}

    private void OnDestroy()
    {
        m_subscriptions.UnsubscribeAll();
    }


    public void AddTimeToToal(Mouledoux.Callback.Packet packet)
    {
        AddTimeToToal(packet.ints[0]);
    }


    public void AddTimeToToal(long time)
    {
        Combu.CombuManager.platform.ReportScore(time, "total_time", (bool success) =>
        { Mouledoux.Components.Mediator.instance.NotifySubscribers("db_total_time", new Mouledoux.Callback.Packet()); });
    }
}
