using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneOnTrigger : MonoBehaviour
{
    public bool InZone;

    private void OnTriggerStay(Collider other)
    {
        if (InZone == false)
        {
            if (other.gameObject == FindObjectOfType<AIGuideBehaviour>().CurrentBase.gameObject)
            {
                InZone = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (InZone == true)
        {
            if (other.gameObject == FindObjectOfType<AIGuideBehaviour>().CurrentBase.gameObject)
            {
                InZone = false;
            }
        }
    }
}
