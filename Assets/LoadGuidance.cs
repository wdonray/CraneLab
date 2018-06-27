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
        transform.LookAt(CalculateWorldPointPosition());
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
}