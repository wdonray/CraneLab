using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayEventOnStart : MonoBehaviour
{
    public float Delay;
    public UnityEngine.Events.UnityEvent _action;

	IEnumerator Start () {
		yield return new WaitForSeconds(Delay);
        _action.Invoke();
        Destroy(this);
	}
}
