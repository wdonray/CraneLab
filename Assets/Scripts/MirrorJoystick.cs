using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorJoystick : MonoBehaviour
{
    public string m_joystick;

	void Update ()
    {
        Vector3 newRotation = Vector3.zero;

        newRotation.x = Input.GetAxis(m_joystick + "_HORIZONTAL");
        newRotation.y = Input.GetAxis(m_joystick + "_VERTICAL");
        newRotation.z = Input.GetAxis(m_joystick + "_ROTATION");
        newRotation *= 20f;


        transform.localEulerAngles = newRotation;
    }
}
