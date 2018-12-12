using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Beginner = 0,
    Intermediate = 1,
    Expert = 2
}

public class DifficultySettings : MonoBehaviour
{
    public static Difficulty CurrentDifficulty;
    public static float SliderValues;

    public void ChangeDifficulty(int difficulty)
    {
        CurrentDifficulty = (Difficulty) difficulty;

        switch (CurrentDifficulty)
        {
            case Difficulty.Beginner:
                SliderValues = .15f;
                break;
            case Difficulty.Intermediate:
                SliderValues = .40f;
                break;
            case Difficulty.Expert:
                SliderValues = 1f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
