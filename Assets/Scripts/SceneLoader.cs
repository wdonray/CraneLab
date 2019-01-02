using System.Collections;
using System.Collections.Generic;
using Combu;
using UnityEngine;

public class SceneLoader : Singleton<SceneLoader>
{
    public long accUp, accDown;
    [HideInInspector] public string onLoadMessage;
    [HideInInspector] public float TimeInScene = 0;
    private RotationToMouseTracking _admin;

    public void SetLoadMessage(string newMessage)
    {
        Instance.onLoadMessage = newMessage;
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnLevelWasLoaded()
    {
        if (onLoadMessage == null) onLoadMessage = "";
        print(onLoadMessage);

        ReportScores();
        SetTimeInTest(0f);

        _admin = null;
        _admin = FindObjectOfType<RotationToMouseTracking>();
        Mouledoux.Components.Mediator.instance.NotifySubscribers(onLoadMessage, new Mouledoux.Callback.Packet());
    }

    public void Update()
    {
        if (_admin)
        {
            TimeInScene += Time.deltaTime;
        }
    }

    public float GetTimeInTest()
    {
        return TimeInScene;
    }

    public void SetTimeInTest(float value)
    {
        TimeInScene = value;
    }

    void ReportScores()
    {
        if ((AccuracyScoreManager.Instance.GetDropOffScore() + AccuracyScoreManager.Instance.GetLoadUpScore()) == 2f)
            return;

        if (CombuManager.isInitialized)
        {
            Combu.CombuManager.platform.ReportScore((long) GetTimeInTest(), "total_time",
                (bool success) =>
                {
                    Mouledoux.Components.Mediator.instance.NotifySubscribers("db_total_time",
                        new Mouledoux.Callback.Packet());
                });

            AccuracyScoreManager.Instance.SimplifyLoad();

            accDown = (long) (AccuracyScoreManager.Instance.GetDropOffGraded() * 100f);
            accUp = (long) (AccuracyScoreManager.Instance.GetLoadUpGraded() * 100f);

            Combu.CombuManager.platform.ReportScore((accDown), "accuracy_dropoff",
                (bool success) =>
                {
                    Debug.Log("<color=yellow> [Score Reported] Drop Off: " + accDown + "</color>");
                    Mouledoux.Components.Mediator.instance.NotifySubscribers("db_accuracy_dropoff",
                        new Mouledoux.Callback.Packet());
                });

            Combu.CombuManager.platform.ReportScore((accUp), "accuracy_pickup",
                (bool success) =>
                {
                    Debug.Log("<color=yellow> [Score Reported] Pick Up: " + accUp + "</color>");
                    Mouledoux.Components.Mediator.instance.NotifySubscribers("db_accuracy_pickup",
                        new Mouledoux.Callback.Packet());
                });
            
        }
    }
}