using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationToInputAxis : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z,
    }
    public Axis m_rotationAroundAxis;

    public string m_inputAxisName;

    public float m_speedMod = 1f;
    public float m_dragMod = 0.1f;


    private Vector3 m_currentRotation;
    private Vector3 m_originPos;

    private void Start()
    {
        m_currentRotation = Vector3.zero;
    }

    void Update ()
    {
        Vector3 rotationEulerAngles = Vector3.zero;

		switch(m_rotationAroundAxis)
        {
            case Axis.X:
                rotationEulerAngles.x = Input.GetAxis(m_inputAxisName);
                break;


            case Axis.Y:
                rotationEulerAngles.y = Input.GetAxis(m_inputAxisName);
                break;


            case Axis.Z:
                rotationEulerAngles.z = Input.GetAxis(m_inputAxisName);
                break;


            default:
                break;
        }

        float throttleMod = (Mathf.Abs(Input.GetAxis("RIGHT_ROTATION"))) + 0.1f;// - 1) / 2) + 0.1f;
        throttleMod *= Time.deltaTime;

        m_currentRotation +=
            rotationEulerAngles.magnitude > 0 ?
            (rotationEulerAngles * (m_speedMod * throttleMod)) :
            Vector3.zero;

        m_currentRotation -= (m_currentRotation * (m_dragMod * Time.deltaTime));
            
        
        transform.Rotate(m_currentRotation);
	}
}
