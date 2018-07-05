using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HookLoop : MonoBehaviour
{
    Rigidbody m_hook;
    HingeJoint m_connectionJoint;

    float grabTimer = 0;

    void Update()
    {
        grabTimer -= Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (m_hook != null || grabTimer > 0) return;

        else if(other.CompareTag("Hook"))
        {
            m_hook = other.GetComponent<Rigidbody>();
            m_connectionJoint = gameObject.AddComponent<HingeJoint>();
            m_connectionJoint.connectedBody = m_hook;
            m_connectionJoint.autoConfigureConnectedAnchor = false;
            m_connectionJoint.anchor = new Vector3(0, 0, -0.75f);
            m_connectionJoint.connectedAnchor = new Vector3(0, 0, -0.5f);

            JointLimits newLimits = new JointLimits();
            newLimits.min = -60f;
            newLimits.max = 60f;
            m_connectionJoint.useLimits = true;
            m_connectionJoint.limits = newLimits;
        }
    }


    public void Drop()
    {
        m_hook = null;
        Destroy(m_connectionJoint);
        grabTimer = 3f;
    }
}
