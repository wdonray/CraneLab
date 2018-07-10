using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float time;
    public List<Text> timerText = new List<Text>();

    public UnityEngine.Events.UnityEvent onTimerEnd;

    [Space]

    public bool hasTriggered = false;

	// Update is called once per frame
	void Update ()
    {
        if (hasTriggered) return;

        time -= Time.deltaTime;

        foreach (Text t in timerText)
        {
            t.text = Mathf.Floor(time / 60f).ToString("00") + ":" + (time % 60f).ToString("00");
        }

        if (time <= 0f)
        {
            onTimerEnd.Invoke();
            hasTriggered = true;
        }
	}
}
