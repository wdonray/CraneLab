using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLoop : MonoBehaviour
{
    Transform m_hook;

    Vector3 m_lastPos;
    Vector3 m_velocity;

	// Use this for initialization
	void Start ()
    {
        m_lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_hook != null)
        {
            Vector3 d2h = m_hook.position - transform.position;
            d2h *= Time.deltaTime;
            transform.position = (m_hook.position);
            transform.rotation = m_hook.rotation;

            if (Input.GetButtonDown("RIGHT_TRIGGER"))
            {
                Drop();
            }
        }
    }

    private void FixedUpdate()
    {
        m_velocity = transform.position - m_lastPos;
        m_lastPos = transform.position;

        print(m_velocity);
    }

    public void Drop()
    {
        m_hook = null;
        GetComponent<Rigidbody>().velocity = m_velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_hook != null) return;


        else
        {
        print("hook");
            if(other.CompareTag("Hook"))
            {
                m_hook = other.transform;
            }
        }
    }
}
