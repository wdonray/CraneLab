using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToAnimator
{
    private static Animator m_animator;

    public static void SendTrigger(GameObject sender, string value)
    {
        m_animator = sender.GetComponent<Animator>();
        m_animator.SetTrigger(value);
    }
}
