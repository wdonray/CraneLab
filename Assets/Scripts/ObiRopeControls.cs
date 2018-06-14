using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiRopeCursor))]
public class ObiRopeControls : MonoBehaviour
{
    public string axis;
    public float reelSpeed;

    ObiRope rope;
    ObiRopeCursor cursor;

    float ropeLength;

    // Use this for initialization
    void Start()
    {
        cursor = GetComponent<ObiRopeCursor>();
        rope = cursor.GetComponent<ObiRope>();

        ropeLength = rope.RestLength;
    }

    void Update()
    {
        if (Input.GetAxis(axis) != 0f)
        {
            ropeLength -= Input.GetAxis(axis) * reelSpeed * Time.deltaTime;
            cursor.ChangeLength(ropeLength);
            //cursor.normalizedCoord = 0.8f;
        }
	}
}
