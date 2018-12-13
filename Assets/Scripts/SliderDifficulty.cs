using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderDifficulty : MonoBehaviour {
    
	void Awake ()
	{
	    foreach (var slider in GetComponentsInChildren<Slider>())
	    {
	        slider.value = DifficultySettings.SliderValues;
	    }
	}
}
