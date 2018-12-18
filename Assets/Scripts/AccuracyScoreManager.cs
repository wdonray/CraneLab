using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyScoreManager : MonoBehaviour
{
    private float _loadMaxScore, _dropMaxScore;

    [SerializeField] private float _loadUpScore, _dropOffScore;
    [Space] [SerializeField] private float LoadGrade, DropGrade;
    private float _threshold;

    private static AccuracyScoreManager _instance;

    public static AccuracyScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AccuracyScoreManager>();

                if (_instance == null)
                {
                    var singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<AccuracyScoreManager>();
                    singletonObject.name = "----- " + typeof(AccuracyScoreManager) + " -----";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        _loadMaxScore = 100;
        _dropMaxScore = 100;
        if (Instance != this) Destroy(gameObject);
    }

    void Update()
    {
        LoadGrade = GetLoadUpScoreGraded();
        DropGrade = GetDropOffGraded();
    }

    public float GetLoadUpScore()
    {
        return _loadUpScore;
    }

    public float GetDropOffScore()
    {
        return _dropOffScore;
    }

    public void AddToLoadUp(float value, float maxValue)
    {
        _loadUpScore += value;
        _loadMaxScore += maxValue;
    }

    public void AddToDropOff(float value, float maxValue)
    {
        _dropOffScore += value;
        _dropMaxScore += maxValue;
    }

    public float GetLoadUpScoreGraded()
    {
        return Round(GetLoadUpScore() / _loadMaxScore, 2);
    }

    public float GetDropOffGraded()
    {
        return Round(GetDropOffScore() / _dropMaxScore, 2);
    }

    public void SimplifyLoad()
    {
        SetDropOffScore(GetDropOffScore() / _dropMaxScore);
        _dropMaxScore = 1f;

        SetLoadUpScore(GetLoadUpScore() / _loadMaxScore);
        _loadMaxScore = 1f;
    }

    public void SetLoadUpScore(float value)
    {
        _loadUpScore = value;
    }

    public void SetDropOffScore(float value)
    {
        _dropOffScore = value;
    }

    public void SetThreshold(float value)
    {
        _threshold = value;
    }

    public float CheckAccuaracy(Vector2 active, Vector2 target, float weight)
    {
        return weight - (weight * Mathf.Clamp01(Vector2.Distance(active, target)));
    }

    private float Round(float value, int digits)
    {
        var mult = Mathf.Pow(10.0f, (float) digits);
        return Mathf.Round(value * mult) / mult;
    }
}
