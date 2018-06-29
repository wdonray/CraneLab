using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGuidance : MonoBehaviour
{
    public Transform m_load;
    public Transform m_dropZone;
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(Maxamize(CalculateWorldPointPosition()));
	}

    private Vector3 CalculateWorldPointPosition()
    {
        if (m_load == null || m_dropZone == null) return Vector3.zero;

        Vector3 targetWorldPos = Vector3.zero;

        targetWorldPos = m_dropZone.position - m_load.position;
        targetWorldPos.Normalize();
        targetWorldPos += transform.position;

        return targetWorldPos;
    }

    private Vector3 Maxamize(Vector3 original)
    {
        Vector3 max = original;
        Vector3 absOrig = new Vector3(Mathf.Abs(max.x), Mathf.Abs(max.y), Mathf.Abs(max.z));
        float highestValue = 0f;

        highestValue = absOrig.x > highestValue ? absOrig.x : highestValue;
        highestValue = absOrig.y > highestValue ? absOrig.y : highestValue;
        highestValue = absOrig.z > highestValue ? absOrig.z : highestValue;

        max.x = max.x == highestValue ? 1 : 0;
        max.y = max.y == highestValue ? 1 : 0;
        max.z = max.z == highestValue ? 1 : 0;

        return max;
    }
}