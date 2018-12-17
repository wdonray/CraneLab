using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUBManager : MonoBehaviour
{
    public Text NameText, TotalVRTime, DropText, PickUpText;
	// Use this for initialization
	void Start ()
	{
	    TotalVRTime.text = "Total VR Time: " + TimeSpan.FromSeconds(Round(SceneLoader._instance.GetTimeInTest(), 0)).ToString("");
	    var DropGrade = AccuracyScoreManager.Instance.GetDropOffGraded();
	    var LoadGrade = AccuracyScoreManager.Instance.GetLoadUpScoreGraded();
        DropText.text = DropGrade + "%";
	    PickUpText.text = LoadGrade + "%";
    }

    public void SetNameText(string value)
    {
        NameText.text = "Welcome Back," + value;
    }

    private float Round(float value, int digits)
    {
        var mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
}
