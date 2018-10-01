using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public static string onLoadMessage;

    private static SceneLoader _instance;
    public static SceneLoader instance
    {
        get
        {
            if (_instance = null) FindObjectOfType<SceneLoader>();
            return _instance;
        }
    }


    public static void SetLoadMessage(string newMessage)
    {
        onLoadMessage = newMessage;
    }


    private void Awake()
    {
        if (instance != this) Destroy(this);

        if (onLoadMessage == null) onLoadMessage = "";
        Mouledoux.Components.Mediator.instance.NotifySubscribers(onLoadMessage, new Mouledoux.Callback.Packet());
        onLoadMessage = null;
    }
}