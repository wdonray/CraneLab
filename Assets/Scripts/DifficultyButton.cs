using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour {
    public Difficulty ButtonDifficulty;
    [HideInInspector] public Button TheButton => GetComponent<Button>();

    void Awake () {
        switch (name)
        {
            case "B":
                ButtonDifficulty = Difficulty.Beginner;
                break;
            case "I":
                ButtonDifficulty = Difficulty.Intermediate;
                break;
            case "E":
                ButtonDifficulty = Difficulty.Expert;
                break;
        }
	}
}
