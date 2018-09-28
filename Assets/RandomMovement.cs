using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public UnityEngine.UI.Slider strengthSlider;

    public Vector3 initPosition;
    public Vector3 randomPos;
    public Vector3 newPos;



	void Start ()
    {
        initPosition = transform.position;
        newPos = initPosition;
	}

    float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = initPosition;
        newPos.y += Mathf.Cos(Time.time) * strengthSlider.value;

        Vector3 newEulers = new Vector3(0.1f, 1f, 5f) * Mathf.Sin(Time.time) * strengthSlider.value;
        Quaternion newRot = Quaternion.Euler(newEulers);

        transform.localPosition = newPos;
        transform.localRotation = newRot;
        
    }
}