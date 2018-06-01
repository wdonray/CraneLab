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

        rotationEulerAngles *= (m_speedMod * Time.deltaTime);
        transform.Rotate(rotationEulerAngles);
	}
}
