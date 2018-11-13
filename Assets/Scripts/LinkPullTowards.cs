using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPullTowards : MonoBehaviour
{
    public float PullForce;

    private void Awake()
    {
        PullForce = 400;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag != "Hook") return;
        var forceDirection = transform.position -  other.transform.position;
        float dist = Vector3.Distance(transform.position, other.transform.position);
        dist /= GetComponent<SphereCollider>().radius;
        other.GetComponent<Rigidbody>().AddForce(forceDirection.normalized * (PullForce * dist)); 
    }
}