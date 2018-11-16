﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;


public class HookLoop : MonoBehaviour
{
    Rigidbody m_hook;
    HingeJoint m_connectionJoint;
    public bool CanAutoHook = false;
    float grabTimer = 0;
    [HideInInspector] public bool Hooked;
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
        //onPickUp += AIGuideBehaviour.GuideWalkBool;
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
        StartCoroutine(HookUp(other));
    }


    public IEnumerator HookUp(Collider other)
    {
        if (m_hook != null || grabTimer > 0) yield break;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        while (Vector3.Distance(transform.position, other.transform.position) >= .1f)
        {
            transform.position = Vector3.Lerp(transform.position, other.transform.position, Time.deltaTime * 50f);
            yield return null;
        }

        if (Vector3.Distance(transform.position, other.transform.position) <= .1f)
        {
            Hooked = true;
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
            m_connectionJoint.enableCollision = false;
            rb.isKinematic = false;
        }
    }


    public void Drop()
    {
        m_hook = null;
        Hooked = false;
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
