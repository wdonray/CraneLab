using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLoop : MonoBehaviour
{
    Transform m_hook;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(m_hook != null)
        {
            transform.position = m_hook.position;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            m_hook = null;
            SetGravity(true);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Hook"))
        {
            m_hook = other.transform;
            SetGravity(false);
        }

        else if(other.gameObject.CompareTag("DropZone"))
        {
            m_hook = null;
            SetGravity(true);
        }
    }

    void SetGravity(bool isEnabled)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) return;

        rb.useGravity = isEnabled;
        rb.isKinematic = !isEnabled;

        if (isEnabled)
            rb.AddForce(rb.velocity);
    }
}
