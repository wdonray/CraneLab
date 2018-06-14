using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLoop : MonoBehaviour
{
    Rigidbody m_hook;
    HingeJoint m_connectionJoint;



    void Update()
    {
        if (m_hook != null)
        {
            if (Input.GetButtonDown("RIGHT_TRIGGER"))
            {
                Drop();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (m_hook != null) return;

        else if(other.CompareTag("Hook"))
        {
            m_hook = other.GetComponent<Rigidbody>();
            m_connectionJoint = gameObject.AddComponent<HingeJoint>();
            m_connectionJoint.connectedBody = m_hook;
        }
    }


    public void Drop()
    {
        m_hook = null;
        Destroy(GetComponent<HingeJoint>());
    }
}
