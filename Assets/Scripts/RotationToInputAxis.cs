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

    public bool m_restrictRotation;

    public float m_rotationMin;
    public float m_rotationMax;

    private Vector3 m_currentRotation;
    private Vector3 m_originPos;

    private void Start()
    {
        m_currentRotation = Vector3.zero;
    }

    void Update ()
    {
        Vector3 rotationEulerAngles = Vector3.zero;
        float currentRotationAngle = 0f;

        switch (m_rotationAroundAxis)
        {
            case Axis.X:
                rotationEulerAngles.x = Input.GetAxis(m_inputAxisName);

                currentRotationAngle = transform.localEulerAngles.x;
                currentRotationAngle = (currentRotationAngle > 180) ? currentRotationAngle - 360 : currentRotationAngle;

                if (!m_restrictRotation) break;

                if (currentRotationAngle < m_rotationMin && rotationEulerAngles.x > 0)
                {
                    rotationEulerAngles.x = 0f;
                }

                if (currentRotationAngle > m_rotationMax && rotationEulerAngles.x < 0)
                {
                    rotationEulerAngles.x = 0f;
                }

                break;


            case Axis.Y:
                rotationEulerAngles.y = Input.GetAxis(m_inputAxisName);

                currentRotationAngle = transform.localEulerAngles.y;
                currentRotationAngle = (currentRotationAngle > 180) ? currentRotationAngle - 360 : currentRotationAngle;

                if (!m_restrictRotation) break;

                else if (currentRotationAngle < m_rotationMin && rotationEulerAngles.y > 0)
                {
                    rotationEulerAngles.y = 0f;
                }
                else if (currentRotationAngle > m_rotationMax && rotationEulerAngles.y < 0)
                {
                    rotationEulerAngles.y = 0f;
                }

                break;


            case Axis.Z:
                rotationEulerAngles.z = Input.GetAxis(m_inputAxisName);

                currentRotationAngle = transform.localEulerAngles.z;
                currentRotationAngle = (currentRotationAngle > 180) ? currentRotationAngle - 360 : currentRotationAngle;

                if (!m_restrictRotation) break;
                else if (currentRotationAngle < m_rotationMin && rotationEulerAngles.z > 0)
                {
                    rotationEulerAngles.z = 0f;
                }
                else if (currentRotationAngle > m_rotationMax && rotationEulerAngles.z < 0)
                {
                    rotationEulerAngles.z = 0f;
                }

                break;


            default:
                break;
        }
        
        float throttleMod = (Mathf.Abs(Input.GetAxis("RIGHT_ROTATION"))) + 0.5f;
        throttleMod *= Time.deltaTime;

        m_currentRotation +=
            rotationEulerAngles.magnitude > 0 ?
            (rotationEulerAngles * (m_speedMod * throttleMod)) :
            Vector3.zero;

        m_currentRotation -= (m_currentRotation * (m_dragMod * Time.deltaTime));
        

        transform.Rotate(m_currentRotation);
    }
}
