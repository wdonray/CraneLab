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
        if (other.gameObject == _guideHelper.LoadToZone[GuideHelper.Index].Load.transform.parent.GetChild(2).gameObject)
        {
            InZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _guideHelper.LoadToZone[GuideHelper.Index].Load.transform.parent.GetChild(2).gameObject)
        {
            InZone = false;
        }
    }
}
