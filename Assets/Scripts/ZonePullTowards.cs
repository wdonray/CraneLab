using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePullTowards : MonoBehaviour
{
    public bool Active;
    private float _pullForce;
    private GuideHelper _guideHelper;
    private BoxCollider _boxCollider;

    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _guideHelper = FindObjectOfType<GuideHelper>();
        _pullForce = 2500;
    }

    public void Update()
    {
        foreach (var rigger in _guideHelper.Riggers)
        {
            if (rigger._complete)
            {
                return;
            }
        }
        Active = _guideHelper.Zones[GuideHelper.Index] == transform.parent.gameObject;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.GetChild(0).tag != "Link") return;
        if (!Active) return;
        var dist = Vector3.Distance(transform.position, other.transform.position);
        dist /= (_boxCollider.size.y * transform.lossyScale.y);
        dist = Mathf.Clamp01(dist);
        var adjustedStrength = _pullForce - (dist * _pullForce);
        FindObjectOfType<AIGuideBehaviour>().CurrentBase.GetComponent<Rigidbody>().AddForce(Vector3.down * adjustedStrength);
    }
}
