using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyScoreManager : Singleton<AccuracyScoreManager>
{
    private float _loadMaxScore, _dropMaxScore;

    [SerializeField] private float _loadUpScore, _dropOffScore;

    private float _threshold;

    void Awake()
    {
        if (Instance != this) Destroy(gameObject);

        _loadUpScore =
        _dropOffScore =
        _loadMaxScore =
        _dropMaxScore = 1f;
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

    public float GetLoadUpGraded()
    {
        while (_loadMaxScore > 1f)
        {
            _loadUpScore /= 10f;
            _loadMaxScore /= 10;
        }

        _loadMaxScore = 1f;

        return Round(_loadUpScore / _loadMaxScore, 2);
    }

    public float GetDropOffGraded()
    {
        while (_dropMaxScore > 1f)
        {
            _dropOffScore /= 10f;
            _dropMaxScore /= 10f;
        }

        _dropMaxScore = 1f;

        return Round(_dropOffScore / _dropMaxScore, 2);
    }

    public void SimplifyLoad()
    {
        SetDropOffScore(_dropOffScore / _dropMaxScore);
        _dropMaxScore = 1f;

        SetLoadUpScore(_loadUpScore / _loadMaxScore);
        _loadMaxScore = 1f;
    }

    public void SetLoadUpScore(float value)
    {
        _loadUpScore = value;
        _loadMaxScore = 1f;
    }

    public void SetDropOffScore(float value)
    {
        _dropOffScore = value;
        _dropMaxScore = 1f;
    }

    public void SetThreshold(float value)
    {
        _threshold = value;
    }

    public float CheckAccuracy(Vector2 active, Vector2 target, float weight)
    {
        return weight - (weight * Mathf.Clamp01(Vector2.Distance(active, target)));
    }

    private float Round(float value, int digits)
    {
        var mult = Mathf.Pow(10.0f, (float) digits);
        return Mathf.Round(value * mult) / mult;
    }
}
