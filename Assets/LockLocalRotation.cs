using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockLocalRotation : MonoBehaviour
{
    public bool x, y, z;

    Quaternion initRotation;

    // Use this for initialization
    void Start()
    {
        initRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion newRotation = initRotation;

        newRotation.eulerAngles = new Vector3(
            x ? initRotation.eulerAngles.x : transform.parent.localEulerAngles.x,
            y ? initRotation.eulerAngles.y : transform.parent.localEulerAngles.y,
            z ? initRotation.eulerAngles.z : transform.parent.localEulerAngles.z);

        transform.rotation = newRotation;
    }
}
