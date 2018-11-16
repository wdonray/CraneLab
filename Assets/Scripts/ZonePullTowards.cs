using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePullTowards : MonoBehaviour {

    private float _pullForce;

    void Awake () {
        _pullForce = 5000;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.GetChild(0).tag == "Link")
        {
            other.transform.parent.GetChild(2).GetComponent<Rigidbody>().AddForce(Vector3.down * _pullForce);
        }
	}
}
