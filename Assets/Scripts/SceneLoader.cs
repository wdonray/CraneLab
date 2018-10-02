using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public string onLoadMessage;

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
        Mouledoux.Components.Mediator.instance.NotifySubscribers(onLoadMessage, new Mouledoux.Callback.Packet());
    }
}