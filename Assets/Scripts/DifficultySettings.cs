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

public class DifficultySettings : Singleton<DifficultySettings>
{
    public Difficulty CurrentDifficulty;

    public float SliderValues
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

    public Color DifficultyColor
    {
        get
        {
            switch (CurrentDifficulty)
            {
                case Difficulty.Beginner:
                    return Color.green;
                case Difficulty.Intermediate:
                    return Color.yellow;
                case Difficulty.Expert:
                    return Color.red;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    void Awake()
    {
        if (Instance != this) Destroy(gameObject);
    }

    public void ChangeDifficulty(int difficulty) => CurrentDifficulty = (Difficulty)difficulty;
}
