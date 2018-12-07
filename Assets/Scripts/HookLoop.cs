using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;


public class HookLoop : MonoBehaviour
{
    private LinkPullTowards magnet => GetComponent<LinkPullTowards>();
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
        subscription.Subscribe("drop", onDrop);
        //subscription.Subscribe(transform.GetInstanceID().ToString(), onPickUp);
    }

    void Update()
    {
        grabTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hook") || !CanAutoHook) return;
        HookUpZTest(other);
    }

    private IEnumerator coroutine;
    public void HookUp(Collider other)
    {
        if (m_hook != null || grabTimer > 0) return;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        if (magnet.Pulling)
        {
            magnet.enabled = false;
            Hooked = true;
            m_hook = other.GetComponent<Rigidbody>();
            m_hook.angularDrag = 20;

            coroutine = LookUp(m_hook.transform);
            StartCoroutine(coroutine);

            m_connectionJoint = gameObject.AddComponent<HingeJoint>();
            m_connectionJoint.connectedBody = m_hook;
            m_connectionJoint.autoConfigureConnectedAnchor = false;
            m_connectionJoint.anchor = new Vector3(0, 0, 0);
            m_connectionJoint.connectedAnchor = new Vector3(0, 0, -0.75f);

            JointLimits newLimits = new JointLimits();
            newLimits.min = -60f;
            newLimits.max = 60f;
            m_connectionJoint.useLimits = true;
            m_connectionJoint.limits = newLimits;
            m_connectionJoint.enableCollision = false;
            rb.isKinematic = false;
        }
    }

    public void HookUpZTest(Collider other)
    {
        if (m_hook != null || grabTimer > 0) return;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
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


    public void Drop()
    {
        if (m_hook != null)
        {
            m_hook.angularDrag = 1;
            m_hook = null;
        }

        Hooked = false;
        if (m_connectionJoint != null)
        {
            Destroy(m_connectionJoint);
            if (coroutine != null)
                StopCoroutine(coroutine);
            gameObject.SetActive(false);
        }

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

    private IEnumerator LookUp(Transform obj)
    {
        while (true)
        { 
            var pos = Vector3.forward - obj.position;
            var newRot = Quaternion.LookRotation(pos);
            obj.rotation = Quaternion.Lerp(obj.rotation, newRot, Time.deltaTime / 3f);
            yield return new WaitForEndOfFrame();
        }
    }
}
