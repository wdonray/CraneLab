using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    public Difficulty ButtonDifficulty;
    [HideInInspector] public Button TheButton => GetComponent<Button>();

    void Awake()
    {
        switch (name)
        {
            case "B":
                ButtonDifficulty = Difficulty.Beginner;
                GetComponent<Image>().color = Color.green;
                break;
            case "I":
                ButtonDifficulty = Difficulty.Intermediate;
                GetComponent<Image>().color = Color.yellow;
                break;
            case "E":
                ButtonDifficulty = Difficulty.Expert;
                GetComponent<Image>().color = Color.red;
                break;
        }
        TheButton.onClick.AddListener(() => DifficultySettings.Instance.ChangeDifficulty((int)ButtonDifficulty));
    }
}
