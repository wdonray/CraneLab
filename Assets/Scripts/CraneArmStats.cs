using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CraneArmStats : MonoBehaviour
{

    public Transform m_armRoot;
    public float m_armLength;

    public float armRotationDeg
    {
        get { return 360f % m_armRoot.localEulerAngles.x; }
    }
    
    public float armRadius
    {
        get { return m_armLength * (Mathf.Cos(armRotationDeg * Mathf.Deg2Rad)); }
    }

    [SerializeField]
    private UnityEngine.UI.Text distanceDispay;
    [SerializeField]
    private UnityEngine.UI.Text rotationDisplay;


    void Start ()
    {
        m_armRoot = m_armRoot == null ? transform : m_armRoot;
	}
	
	void Update ()
    {
        distanceDispay.text = armRadius.ToString("0") + "'";
        rotationDisplay.text = armRotationDeg.ToString("0") + "°";
    }
}
