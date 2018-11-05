using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HookLoop : MonoBehaviour
{
    Rigidbody m_hook;
    HingeJoint m_connectionJoint;

    float grabTimer = 0;

    //Mouledoux.Components.Mediator.Subscriptions subscription = new Mouledoux.Components.Mediator.Subscriptions();
    //Mouledoux.Callback.Callback onDrop;

    //private void Awake()
    //{
    //    OnEnable();
    //}

    //private void OnEnable()
    //{
    //    onDrop += Drop;
    //    subscription.Subscribe("drop", onDrop);
    //}

    void Update()
    {
        grabTimer -= Time.deltaTime;
    }


    public void HookUp(Collider other)
    {
        if (m_hook != null || grabTimer > 0) return;

        else if(other.CompareTag("Hook"))
        {
            transform.position = Vector3.Lerp(transform.position, other.transform.position, Time.deltaTime * 1f);

            if (Vector3.Distance(transform.position, other.transform.position) <= 1f)
            {
                m_hook = other.GetComponent<Rigidbody>();
                m_connectionJoint = gameObject.AddComponent<HingeJoint>();
                m_connectionJoint.connectedBody = m_hook;
                m_connectionJoint.autoConfigureConnectedAnchor = false;
                m_connectionJoint.anchor = new Vector3(0, 0, 0);
                m_connectionJoint.connectedAnchor = new Vector3(0, 0, -0.5f);

                JointLimits newLimits = new JointLimits();
                newLimits.min = -60f;
                newLimits.max = 60f;
                m_connectionJoint.useLimits = true;
                m_connectionJoint.limits = newLimits;
            }
        }
    }


    public void Drop()
    {
        m_hook = null;

        if(m_connectionJoint != null)
            Destroy(m_connectionJoint);

        grabTimer = 3f;
    }

    //public void Drop(Mouledoux.Callback.Packet packet)
    //{
    //    Drop();
    //}

    //private void OnDestroy()
    //{
    //    subscription.UnsubscribeAll();
    //}
}
