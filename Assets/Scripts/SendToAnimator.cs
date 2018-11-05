using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToAnimator
{
    public static bool stop;
    private static string m_oldValue;

    public static void SendTrigger(GameObject sender, string value)
    {
        var m_animator = sender.GetComponent<Animator>();
        var m_aiGuide = m_animator.gameObject.GetComponent<AIGuideBehaviour>();

        if (!m_aiGuide.m_tieOnly)
        {
            if (stop)
            {
                return;
            }
        }

        if (m_animator.IsInTransition(0))
        {
            return;
        }

        if (m_aiGuide.m_dead)
        {
            m_animator.SetTrigger("Death");
        }

        if (m_oldValue == value)
        {
            return;
        }

        m_oldValue = value;
        m_animator.SetTrigger(m_oldValue);
        Debug.Log(m_oldValue);
    }

    public static void SendTriggerForce(GameObject sender, string value)
    {
        var m_animator = sender.GetComponent<Animator>();
        if (value == m_oldValue) return;
        m_animator.StopPlayback();
        m_animator.SetTrigger(value);
        m_oldValue = value;
    }

    public static void ResetTrigger(GameObject sender, string value)
    {
        var m_animator = sender.GetComponent<Animator>();
        m_animator.ResetTrigger(value);
    }

    public static void ResetAllTriggers(GameObject sender)
    {
        var m_animator = sender.GetComponent<Animator>();
        foreach (AnimatorControllerParameter parameter in m_animator.parameters)
        {
            m_animator.ResetTrigger(parameter.name);
        }
    }
}

//var m_aiGuide = m_animator.gameObject.GetComponent<AIGuideBehaviour>();

//if (m_aiGuide.m_dead)
//{
//    m_animator.SetTrigger("Death");
//}