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
    public Difficulty CurrentDifficulty;
    private static DifficultySettings _instance;

    public static DifficultySettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DifficultySettings>();

                if (_instance == null)
                {
                    var singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<DifficultySettings>();
                    singletonObject.name = "----- " + typeof(DifficultySettings) + " -----";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (Instance != this) Destroy(gameObject);
    }

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

    public void ChangeDifficulty(int difficulty) => CurrentDifficulty = (Difficulty)difficulty;
}
