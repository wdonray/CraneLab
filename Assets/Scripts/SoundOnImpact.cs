using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnImpact : MonoBehaviour
{
    public float forceThreshold;
    public AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > forceThreshold)
            audioSource.PlayOneShot(audioSource.clip);
    }
}
