using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderToInputAxis : MonoBehaviour
{
    private Slider m_slider;

    [SerializeField]
    private string m_axis;
    [SerializeField]
    private bool m_absolute;



	void Start ()
    {
        m_slider = GetComponent<Slider>();
	}
	
    
	void Update ()
    {
        float value = Input.GetAxis(m_axis);
        value = m_absolute ? Mathf.Abs(value) : value;
        m_slider.value = value;
	}
}
