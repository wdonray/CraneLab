using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPullTowards : MonoBehaviour
{
    private float _pullForce;

    private void Awake()
    {
        _pullForce = 400;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag != "Hook") return;
        var forceDirection = transform.position -  other.transform.position;
        float dist = Vector3.Distance(transform.position, other.transform.position);
        dist /= GetComponent<CapsuleCollider>().radius;
        other.GetComponent<Rigidbody>().AddForce(forceDirection.normalized * (_pullForce * dist)); 
    }
}