using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public string onLoadMessage;
    public float TimeInScene = 0;
    private RotationToMouseTracking _admin;


    public static SceneLoader _instance;

    public SceneLoader instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<SceneLoader>();
            return _instance;
        }
    }


    public void SetLoadMessage(string newMessage)
    {
        instance.onLoadMessage = newMessage;
    }

    private void Awake()
    {
        if (instance != this)
        {
            print(instance.gameObject.name);
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded()
    {
        if (onLoadMessage == null) onLoadMessage = "";
        print(onLoadMessage);

        OnDestroy();
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

    void OnDestroy()
    {
        Combu.CombuManager.platform.ReportScore((long) GetTimeInTest(), "total_time",
            (bool success) =>
            {
                Mouledoux.Components.Mediator.instance.NotifySubscribers("db_total_time",
                    new Mouledoux.Callback.Packet());
            });

        //if (AccuracyScoreManager.Instance.GetLoadUpScore() != 0 && AccuracyScoreManager.Instance.GetDropOffScore() != 0)
        {
            AccuracyScoreManager.Instance.SimplifyLoad();

            Combu.CombuManager.platform.ReportScore((long) (AccuracyScoreManager.Instance.GetDropOffGraded() * 100),
                "accuracy_dropoff",
                (bool success) =>
                {
                    Mouledoux.Components.Mediator.instance.NotifySubscribers("db_accuracy_dropoff",
                        new Mouledoux.Callback.Packet());
                });

            Combu.CombuManager.platform.ReportScore((long) (AccuracyScoreManager.Instance.GetLoadUpGraded() * 100),
                "accuracy_pickup",
                (bool success) =>
                {
                    Mouledoux.Components.Mediator.instance.NotifySubscribers("db_accuracy_pickup",
                        new Mouledoux.Callback.Packet());
                });
        }
    }
}