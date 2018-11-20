using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToAnimator
{
    public static bool stop;
    public static string m_oldValue, m_oldValueForce;
    public static bool sentOnce;

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
        m_aiGuide.AnimationPlaying = value;
        //Debug.Log(m_oldValue);
    }

    public static void SendTriggerForce(GameObject sender, string value)
    {
        var m_animator = sender.GetComponent<Animator>();
        var m_aiGuide = m_animator.gameObject.GetComponent<AIGuideBehaviour>();

        if ((value + sender.name) == (m_oldValueForce + sender.name))
        {
            return;
        }

        m_animator.StopPlayback();
        m_oldValueForce = value;
        m_animator.SetTrigger(m_oldValueForce);
        m_aiGuide.AnimationPlaying = value;
        //Debug.Log(sender.name + " Forced: " + value);
    }

    public static void SendTriggerForceContinues(GameObject sender, string value)
    {
        var m_animator = sender.GetComponent<Animator>();
        var m_aiGuide = m_animator.gameObject.GetComponent<AIGuideBehaviour>();
        m_animator.SetTrigger(value);
        m_aiGuide.AnimationPlaying = value;
        //Debug.Log(sender.name + " Test Forced: " + value);
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

    public static void SendTriggerOnce(GameObject sender, string value)
    {
        var m_animator = sender.GetComponent<Animator>();
        var m_aiGuide = m_animator.gameObject.GetComponent<AIGuideBehaviour>();
        if (sentOnce == false)
        {
            sentOnce = true;
            m_animator.SetTrigger(value);
            m_aiGuide.AnimationPlaying = value;
           // Debug.Log("Played Once:" + value);
        }
    }
}

//var m_aiGuide = m_animator.gameObject.GetComponent<AIGuideBehaviour>();

//if (m_aiGuide.m_dead)
//{
//    m_animator.SetTrigger("Death");
//}