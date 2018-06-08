using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiRopeCursor))]
public class ObiRopeControls : MonoBehaviour
{
    public string axis;

    ObiRope rope;
    ObiRopeCursor cursor;

    // Use this for initialization
    void Start()
    {
        cursor = GetComponent<ObiRopeCursor>();
        rope = cursor.GetComponent<ObiRope>();
    }

    void Update()
    {
        cursor.ChangeLength(rope.RestLength + -Input.GetAxis(axis) * Time.deltaTime);
	}
}
