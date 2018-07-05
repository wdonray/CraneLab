using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionAdjustment : MonoBehaviour
{
    static Vector3 savedOffset = Vector3.zero;

    private void Start()
    {
        transform.position += savedOffset;
    }

    void Update ()
    {
        if (Input.GetKey(KeyCode.LeftShift) ==
            Input.GetKey(KeyCode.RightShift)) return;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool vertical = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        Vector3 translation = Vector3.zero;
        translation.x = input.x;
        translation.y = vertical ? input.y : 0;
        translation.z = vertical ? 0 : input.y;

        translation *= Time.deltaTime;

        transform.Translate(translation);

        savedOffset += translation;
    }
}