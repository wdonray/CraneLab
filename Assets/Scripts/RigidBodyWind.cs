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

    public UnityEngine.UI.Slider m_slider;

    public float m_streangth;
    public float m_noise;

	void Start ()
    {
        GetComponent<SphereCollider>().isTrigger = true;
	}

    private void OnEnable()
    {
        m_slider.interactable = enabled;
    }
    private void OnDisable()
    {
        m_slider.interactable = enabled;
    }

    void FixedUpdate ()
    {
        if (m_zoneType != eZoneType.Directional) return;

        else
        {
            Vector3 directionalForce = transform.forward * (m_streangth * m_slider.value);
            //directionalForce *= (Random.Range(-m_noise, m_noise));

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

    private void OnDrawGizmos()
    {
        Vector3 backPos = transform.position - transform.forward;
        Vector3 frontPos = transform.position + transform.forward;

        backPos += transform.up / 2f;
        Debug.DrawLine(frontPos, backPos, Color.green);

        backPos -= transform.up;
        Debug.DrawLine(frontPos, backPos);

        backPos += transform.up / 2f;
        backPos += transform.right / 2f;
        Debug.DrawLine(frontPos, backPos, Color.red);

        backPos -= transform.right;
        Debug.DrawLine(frontPos, backPos);
    }
}