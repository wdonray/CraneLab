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

    // Update is called once per frame
    void Update()
    {
        //newPos = Vector3.Lerp(initPosition, randomPos, Mathf.Sin(Time.time));

        //if (Vector3.Distance(initPosition, newPos) < 0.1f)
        //{
        //    randomPos = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * noise;
        //}

        //Vector3 dir = newPos - initPosition;
        //dir.Normalize();

        transform.Translate(Vector3.up * Time.deltaTime * noise.value * Mathf.Sin(Time.time));
    }
}
