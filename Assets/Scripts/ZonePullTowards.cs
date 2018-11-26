using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePullTowards : MonoBehaviour
{

    public bool Active;
    private float _pullForce;
    private GuideHelper _guideHelper => FindObjectOfType<GuideHelper>();
    void Awake()
    {
        _pullForce = 2500;
    }

    public void Update()
    {
        Active = _guideHelper.LoadToZone[GuideHelper.Index].Zone == transform.parent.gameObject;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.GetChild(0).tag == "Link")
        {
            if (Active)
            {
                other.transform.parent.GetChild(2).GetComponent<Rigidbody>().AddForce(Vector3.down * _pullForce);
            }
        }
    }
}
