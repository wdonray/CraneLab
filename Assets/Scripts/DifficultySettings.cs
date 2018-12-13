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

    public static float SliderValues
    {
        get
        {
            switch (CurrentDifficulty)
            {
                case Difficulty.Beginner:
                    return .15f;
                case Difficulty.Intermediate:
                    return .40f;
                case Difficulty.Expert:
                    return 1f;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void ChangeDifficulty(int difficulty)
    {
        CurrentDifficulty = (Difficulty)difficulty;
    }
}
