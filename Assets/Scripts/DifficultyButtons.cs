using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButtons : MonoBehaviour
{
    private static List<DifficultyButton> _difficulityButtons = new List<DifficultyButton>();
    // Use this for initialization
    void Start()
    {
        foreach (var button in transform.GetComponentsInChildren<DifficultyButton>())
        {
            _difficulityButtons.Add(button);
        }
        ButtonActivated(_difficulityButtons[(int)DifficultySettings.CurrentDifficulty]);
    }

    public void ButtonActivated(DifficultyButton currentButton)
    {
        foreach (var button in _difficulityButtons)
        {
            button.TheButton.interactable = button.ButtonDifficulty != currentButton.ButtonDifficulty;
        }
    }
}
