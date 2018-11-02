using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockLocalRotation : MonoBehaviour
{
    public Vector3 m_lockedRotation;

    Quaternion initRotation;

    // Use this for initialization
    void Start()
    {
        initRotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Quaternion newRotation = Quaternion.Euler(m_lockedRotation);
        transform.rotation = newRotation;
    }
}
