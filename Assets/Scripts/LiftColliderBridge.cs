using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftColliderBridge : MonoBehaviour
{
    private LoadMetrics _listener;

    public void Init(LoadMetrics listener)
    {
        _listener = listener;
    }

    void OnCollisionEnter(Collision other)
    {
        _listener.OnCollisionEnter(other);
    }
}
