using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTTimeScore : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void AddScore(UnityEngine.UI.InputField input)
    {
        Combu.CombuManager.platform.ReportScore(long.Parse(input.text), "total_time", (bool success) => { });
    }

    public void GetScore()
    {
        Combu.CombuManager.platform
    }
}
