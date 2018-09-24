using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public UnityEngine.UI.Slider noise;

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
        transform.Rotate(Vector3.forward, 1f * speed);
    }
}