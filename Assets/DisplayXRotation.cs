using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class DisplayXRotation : MonoBehaviour
{

    public Transform m_target;

    private UnityEngine.UI.Text m_text;


    void Start ()
    {
        m_target = m_target == null ? transform : m_target;
        m_text = GetComponent<UnityEngine.UI.Text>();
	}
	
	void Update ()
    {
        m_text.text = (360 % m_target.localEulerAngles.x).ToString("0");
	}
}
