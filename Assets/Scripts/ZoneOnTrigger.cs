using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneOnTrigger : MonoBehaviour
{
    public bool InZone;
    private GuideHelper _guideHelper;

    private void Awake()
    {
        _guideHelper = FindObjectOfType<GuideHelper>();
    }

    private void OnTriggerStay(Collider other)
    {
        var i = _guideHelper.TestType == TestType.Infinite ? GuideHelper.RandomIndexLoad : GuideHelper.Index;
        if (other.gameObject == _guideHelper.Loads[i].transform.parent.GetChild(2).gameObject)
        {
            InZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var i = _guideHelper.TestType == TestType.Infinite ? GuideHelper.RandomIndexLoad : GuideHelper.Index;
        if (other.gameObject == _guideHelper.Loads[i].transform.parent.GetChild(2).gameObject)
        {
            InZone = false;
        }
    }
}
