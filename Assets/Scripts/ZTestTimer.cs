using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTestTimer : MonoBehaviour
{
    public float Timer;
    private AudioSource _buzzer;

    private void Awake()
    {
        _buzzer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_buzzer.isPlaying)
        {
            Timer += Time.deltaTime;
        }
    }
}
