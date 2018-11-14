using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;


public class HookLoop : MonoBehaviour
{
    Rigidbody m_hook;
    HingeJoint m_connectionJoint;
    public bool CanAutoHook = false;
    float grabTimer = 0;

    Mouledoux.Components.Mediator.Subscriptions subscription = new Mouledoux.Components.Mediator.Subscriptions();
    Mouledoux.Callback.Callback onDrop;
    public Mouledoux.Callback.Callback onPickUp;

    private void Awake()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        onDrop += Drop;
        onPickUp += AIGuideBehaviour.GuideWalkedBool;
        subscription.Subscribe("drop", onDrop);
        subscription.Subscribe(transform.GetInstanceID().ToString(), onPickUp);
    }

    void Update()
    {
        grabTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hook") || !CanAutoHook) return;
        HookUp(other);
    }


    public void HookUp(Collider other)
    {
        if (m_hook != null || grabTimer > 0) return;

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


    public void Drop()
    {
        m_hook = null;

        if (m_connectionJoint != null)
            Destroy(m_connectionJoint);

        grabTimer = 3f;
    }

    public void Drop(Mouledoux.Callback.Packet packet)
    {
        Drop();
    }

    private void OnDestroy()
    {
        subscription.UnsubscribeAll();
    }
}
