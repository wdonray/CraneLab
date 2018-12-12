using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButtons : MonoBehaviour {
    private List<Button> _difficulityButtons = new List<Button>();
	// Use this for initialization
	void Start () {
	    foreach (var button in transform.GetComponentsInChildren<Button>())
	    {
	        _difficulityButtons.Add(button);
	    }       

        ButtonActivated(_difficulityButtons[(int) DifficultySettings.CurrentDifficulty]);
	}

    public void ButtonActivated(Button currentButton)
    {
        currentButton.interactable = false;
        foreach (var button in _difficulityButtons)
        {
            if (button != currentButton)
            {
                button.interactable = true;
            }
        }
    }
}
