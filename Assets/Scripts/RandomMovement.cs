using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public UnityEngine.UI.Slider slider;
    private float sliderStrength;

    public Vector3 initPosition;
    public Vector3 randomPos;
    public Vector3 newPos;

    [Space]

    public Vector3 eulerMovement;

	void Start ()
    {
        initPosition = transform.position;
        newPos = initPosition;

        sliderStrength = slider.value;
	}

    float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        sliderStrength += (slider.value - sliderStrength) * Time.deltaTime;

        Vector3 newPos = initPosition;
        newPos.y += Mathf.Cos(Time.time) * sliderStrength;

        Vector3 newEulers = eulerMovement * Mathf.Sin(Time.time) * sliderStrength;
        Quaternion newRot = Quaternion.Euler(newEulers);

        transform.localPosition = newPos;
        transform.localRotation = newRot;
        
    }
}