﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentRotation : MonoBehaviour
{
    Quaternion initRotation;

	// Use this for initialization
	void Start ()
    {
        //initRotation = transform.localRotation;
        transform.parent = null;
        Destroy(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.localRotation = Quaternion.Inverse(transform.parent.localRotation);
	}
}
