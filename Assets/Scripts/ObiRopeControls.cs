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

        cursor.ChangeLength(8f);
        ropeLength = 8f;
        cursor.normalizedCoord = 0.5f;

        //ropeLength = rope.RestLength;
    }

    void Update()
    {
        if (Input.GetAxis(axis) != 0f)
        {
            float input = Input.GetAxis(axis) * (Mathf.Abs(Input.GetAxis("RIGHT_ROTATION")) + 1);
            ropeLength -= input * reelSpeed * Time.deltaTime;
            cursor.ChangeLength(ropeLength);
            //cursor.normalizedCoord = 0.5f;
        }
	}
}
