using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RigidBodyWind : MonoBehaviour
{
    enum eZoneType
    {
        Directional,
        Area,
    }

    [SerializeField]
    private eZoneType m_zoneType;

    public float m_streangth;

	void Start ()
    {
        GetComponent<SphereCollider>().isTrigger = true;
	}
	
	void FixedUpdate ()
    {
        if (m_zoneType != eZoneType.Directional) return;

        else
        {
            Vector3 directionalForce = transform.forward * m_streangth;

            foreach(Rigidbody rb in FindObjectsOfType<Rigidbody>())
            {
                rb.AddForce(directionalForce, ForceMode.Force);
            }
        }
	}

    private void OnTriggerStay(Collider other)
    {
        if (m_zoneType != eZoneType.Area) return;

        else
        {
            Rigidbody otherRB = other.GetComponent<Rigidbody>();
            if (otherRB == null) return;

            Vector3 relativeAreaForce = (other.transform.position - transform.position).normalized * m_streangth;

            otherRB.AddForce(relativeAreaForce, ForceMode.Force);
        }
    }
}
