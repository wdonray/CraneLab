using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public static string onLoadMessage;

    public void SetLoadMessage(string newMessage)
    {
        onLoadMessage = newMessage;
    }

    private void Awake()
    {
        if (onLoadMessage == null) onLoadMessage = "";
        Mouledoux.Components.Mediator.instance.NotifySubscribers(onLoadMessage, new Mouledoux.Callback.Packet());
        onLoadMessage = null;
    }
}
