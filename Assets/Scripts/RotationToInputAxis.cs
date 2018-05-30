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
        
        transform.Rotate(rotationEulerAngles);
	}
}
