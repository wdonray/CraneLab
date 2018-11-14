using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public UnityEngine.UI.Slider slider;
    private float sliderStrength;

    private Vector3 initPosition;
    private Vector3 initEulers;

    public Vector3 randomPos;
    public Vector3 newPos;

    public float offset;

    [Space]

    public Vector3 eulerMovement;

	void Start ()
    {
        initPosition = transform.localPosition;
        initEulers = transform.localEulerAngles;
        newPos = initPosition;

        sliderStrength = slider.value;
	}

    float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        sliderStrength += (slider.value - sliderStrength) * Time.deltaTime;

        newPos = initPosition;
        newPos.y += Mathf.Cos(Time.time) * sliderStrength;

        Vector3 newEulers = initEulers + (eulerMovement * Mathf.Sin(Time.time + offset) * sliderStrength);
        Quaternion newRot = Quaternion.Euler(newEulers);

        transform.localPosition = newPos;
        transform.localRotation = newRot;
        
    }
}