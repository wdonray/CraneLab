using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentRotation : MonoBehaviour
{
    Quaternion initRotation;

	// Use this for initialization
	void Start ()
    {
        initRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.rotation = initRotation * Quaternion.Inverse(transform.parent.localRotation);
	}
}
