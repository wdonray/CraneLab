/* * * * *
 * Creator: Eric Z Mouledoux
 * Contact: Eric@TantrumLab.com
 * 
 * Summary:
 * 
 * 
 * Usage:
 * 
 * 
 * Notes:
 * 
 * 
 * * * * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    /// <summary>
    /// Timer for delaying event excution
    /// </summary>
    private float m_Timer = 0;

    /// <summary>
    /// The current event (the one that just ran)
    /// </summary>
    private TriggeredEvent m_CurrentEvent;

    /// <summary>
    /// List of all the events
    /// </summary>
    public List<TriggeredEvent> m_Events
        = new List<TriggeredEvent>();

    private Mouledoux.Components.Mediator.Subscriptions sub = new Mouledoux.Components.Mediator.Subscriptions();
    private Mouledoux.Callback.Callback onNotify;


    void Awake()
    {
        m_CurrentEvent = m_Events[0];   // Sets the current event to the first event
        onNotify = TriggerEventAtIndex;

        foreach(TriggeredEvent te in m_Events)
        {
            sub.Subscribe("trigger" + te.EventName.ToLower(), onNotify);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TriggerNextEvent();
            return;
        }


        if (m_CurrentEvent.nextEventDelay < 0)          // If the current delay time is less than 0
        {                                                   //
            return;                                         // break out of the update loop
        }

        else                                            // Else (the delay is >= 0)
        {                                                   //
            m_Timer += Time.deltaTime;                      // Increase timer by delta time

            if(m_Timer >= m_CurrentEvent.nextEventDelay)    // If timer is >= the current delay
            {                                                   //
                TriggerNextEvent();                             // Trigger the next event
                m_Timer = 0;                                    // Reset the timer
            }
        }
    }

    /// <summary>
    /// Runs the next event in the list
    /// </summary>
    [ContextMenu("TriggerNext")]
    public void TriggerNextEvent()
    {
        int cIndex = m_Events.FindIndex(i => i == m_CurrentEvent);  // The next event index is the current index +1
        TriggerEventAtIndex(cIndex + 1);                            // Run the event at the next index
    }

    public void TriggerNextEvent(Mouledoux.Callback.Packet packet)
    {
        int cIndex = m_Events.FindIndex(i => i == m_CurrentEvent);  // The next event index is the current index +1
        TriggerEventAtIndex(cIndex + 1);                            // Run the event at the next index
    }

    /// <summary>
    /// Skips forward or backwards to a specific event
    /// </summary>
    /// <param name="aIndex">Index of the event to call</param>
    public void TriggerEventAtIndex(int aIndex)
    {
        if (aIndex >= m_Events.Count)       // Checks if the index passed is within the list
            return;                             // If not, do nothing

        m_CurrentEvent = m_Events[aIndex];  // Set the current event to the new event
        m_CurrentEvent.RunEvent();          // Run the new event
        m_Timer = 0;
    }

    public void TriggerEventAtIndex(Mouledoux.Callback.Packet packet)
    {
        TriggerEventAtIndex(packet.ints[0]);
    }

    public void OnDestroy()
    {
        sub.UnsubscribeAll();
    }
}

[System.Serializable]
public class TriggeredEvent
{
    public string EventName;

    /// <summary>
    /// Unity event linked to this custom event
    /// </summary>
    public UnityEngine.Events.UnityEvent TriggerEvent;

    /// <summary>
    /// Time to wait after this event to run the next one
    /// </summary>
    public float nextEventDelay;

    /// <summary>
    /// Runs the event linked to this class
    /// </summary>
    /// <returns></returns>
    public int RunEvent()
    {
        TriggerEvent.Invoke();
        return 0;
    }
}