using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPullTowards : MonoBehaviour
{
    [SerializeField] private float _pullForce;
    public bool Pulling;

    private void Awake()
    {
        switch (DifficultySettings.Instance.CurrentDifficulty)
        {
            case Difficulty.Beginner:
                _pullForce = 250;
                break;
            case Difficulty.Intermediate:
                _pullForce = 100;
                break;
            case Difficulty.Expert:
                _pullForce = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag != "Hook") return;
        Pulling = true;
        var forceDirection = transform.position -  other.transform.position;
        var dist = Vector3.Distance(transform.position, other.transform.position);
        dist /= GetComponent<CapsuleCollider>().radius;
        other.GetComponent<Rigidbody>().AddForce(forceDirection.normalized * (_pullForce * dist)); 
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag != "Hook") return;
        Pulling = false;
    }
}