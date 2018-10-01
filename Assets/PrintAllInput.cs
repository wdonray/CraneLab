using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintAllInput : MonoBehaviour
{	
	// Update is called once per frame
	void Update ()
    {
        print(Input.GetJoystickNames());
	}
}
