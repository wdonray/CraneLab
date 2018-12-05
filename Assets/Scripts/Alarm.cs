using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    public AudioClip m_alarmSound;
    public AudioSource m_alarmSource;

    public Light m_warningLight;
    public float m_lightIntensity;
    

    [ContextMenu("Alarm")]
    public void StartAlarm()
    {
        StartCoroutine(_Alarm(true));
    }

    public void StopAlarm()
    {
        StartCoroutine(_StopAlarm());
    }

    public IEnumerator _Alarm(bool loop)
    {
        if (m_alarmSource.isPlaying)
        {
            yield break;
        }

        //StopCoroutine("_Alarm");

        m_alarmSource.Stop();
        m_alarmSource.clip = m_alarmSound;
        m_alarmSource.loop = loop;
        m_alarmSource.Play();

        float timeLoop = 0f;
        do
        {
            timeLoop = Mathf.Sin((m_alarmSource.time / m_alarmSource.clip.length) * (2f * Mathf.PI));
            m_warningLight.intensity = m_lightIntensity * timeLoop;
            yield return null;

        } while (loop || m_alarmSource.isPlaying);

        StopAlarm();

        yield return null;
    }

    public IEnumerator _StopAlarm()
    {
        StopCoroutine("_Alarm");

        if (m_alarmSource.isPlaying)
        {
            m_warningLight.intensity = 0f;
            m_alarmSource.Stop();
            yield return null;
        }

    }
}
