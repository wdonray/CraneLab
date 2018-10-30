using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderToInputAxis : MonoBehaviour
{
    private Slider m_slider;

    [SerializeField]
    private Text m_text;

    [SerializeField]
    private string m_axis;
    [SerializeField]
    private bool m_absolute;

    private float value
    {
        get
        {
            float n = Input.GetAxis(m_axis);
            //n = m_absolute ? Mathf.Abs(value) : n;
            return n;
        }
    }
    private float valueRaw
    {
        get
        {
            float n = Input.GetAxisRaw(m_axis);
            return n;
        }
    }

    void Start ()
    {
        m_slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (m_slider != null)
        {
            m_slider.value = value;
        }
    }

    //private void OnGUI()
    //{
    //    if (m_slider != null)
    //    {
    //        m_slider.value = value;
    //    }

    //    if(m_text != null)
    //    {
    //        m_text.text = ((-valueRaw + 1f) * 0.5f).ToString();
    //    }
    //}
}
