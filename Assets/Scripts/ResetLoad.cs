using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLoad : MonoBehaviour
{
    private Vector3 startPos;
	// Use this for initialization
	void Awake ()
	{
	    startPos = transform.position;
	}
	
	public void ResetLoadPos ()
	{
	    transform.position = startPos;
        transform.GetComponent<HookLoop>().gameObject.SetActive(true);
	}
}
