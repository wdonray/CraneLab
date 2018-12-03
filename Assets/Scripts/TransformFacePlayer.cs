using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFacePlayer : MonoBehaviour
{
    public Transform Head;
    public List<Transform> Transforms = new List<Transform>();

	// Update is called once per frame
	void Update () {

	    foreach (var child in transform.GetComponentsInChildren<Transform>())
	    {
	        if (child != transform && !child.GetComponent<SpriteRenderer>() && !Transforms.Contains(child))
	        {
	            Transforms.Add(child);
	        }
	    }

	    foreach (var transform1 in Transforms)
	    {
	        transform1.LookAt(Head);
	    }
    }
}
