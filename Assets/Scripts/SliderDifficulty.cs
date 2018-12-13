using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderDifficulty : MonoBehaviour {
    
	void Awake ()
	{
	    foreach (var slider in GetComponentsInChildren<Slider>())
	    {
	        if (DifficultySettings.Instance.CurrentDifficulty == Difficulty.Beginner)
	        {
	            if (slider.transform.parent.name.Contains("Wind"))
	            {
	                slider.value = 0f;
	            }
	            else
	            {
	                StartCoroutine(LerpValue(slider, DifficultySettings.Instance.SliderValues));
	            }
            }
	        else
	        {
	            StartCoroutine(LerpValue(slider, DifficultySettings.Instance.SliderValues));
	        }
        }
	}

    private IEnumerator LerpValue(Slider slider, float value)
    {
        while (slider.value != value)
        {
            slider.value = Mathf.Lerp(slider.value, value, Time.deltaTime);
            yield return null;
        }
    }
}
