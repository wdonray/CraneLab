using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTransform : MonoBehaviour
{
    public Transform m_target;

    private void Start()
    {
        m_target = m_target == null ? Camera.main.transform : m_target;
    }

    public void LateUpdate()
    {
        transform.LookAt(m_target);
    }
}
