using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PitchToInputAxis : MonoBehaviour
{
    public string m_inputAxis;

    AudioSource m_audioSource;


	// Use this for initialization
	void Start ()
    {
        m_audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_audioSource.pitch = Input.GetAxis(m_inputAxis) + 0.1f;
	}
}
