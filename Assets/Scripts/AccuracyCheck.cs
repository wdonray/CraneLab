using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyCheck : MonoBehaviour
{
    public static Accuracy LoadUp, OffLoad;
    public float MaxPoints, Threshold;
    // Use this for initialization
    void Start()
    {
        LoadUp = new Accuracy(0, new List<float>());
        OffLoad = new Accuracy(0, new List<float>());
    }

    public float CheckAccuaracy(Vector2 active, Vector3 target, float weight)
    {
        var dist = Vector2.Distance(active, target);
        var clamped = Mathf.Clamp01(dist /= Threshold);
        return weight - (100 * clamped);
    }

    public struct Accuracy
    {
        public float Value;
        public List<float> Scores;

        public Accuracy(float value, List<float> scores)
        {
            Value = value;
            Scores = scores;
        }
    }
}
