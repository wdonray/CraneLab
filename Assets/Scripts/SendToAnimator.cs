using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToAnimator
{
    private static string m_oldValue;
    /// <summary>
    ///     (~) Need To Fix / (*) Think I have fixed
    ///     ~TODO (Tying Up): Maybe if the walk / tying up is sent through here we stop until they are done?
    ///     ~TODO (Hoist / Lower): Just not being called
    ///     *TODO (Stop): Whenever you start scene and the hook and load are lined up AI says stop
    ///     *TODO (Animator): Calling same value over and over
    ///     *TODO (Eric): Change playerPos to cranePos
    ///     *TODO (Eric): Make public setter functions for transforms
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="value"></param>
    public static void SendTrigger(GameObject sender, string value)
    {
        var m_animator = sender.GetComponent<Animator>();
        var m_aiGuide = m_animator.gameObject.GetComponent<AIGuideBehaviour>();

        if (m_oldValue == "")
        {
            m_oldValue = value;
        }
        else if (m_oldValue == value /*|| aiGuide.m_targetReached*/)
        {
            return;
        }
        else
        {
            m_oldValue = value;
            m_animator.SetTrigger(m_oldValue);
        }
    }
}
