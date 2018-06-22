using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEventManager : MonoBehaviour
{
    private List<KeyboardEvent> m_keyEvents = new List<KeyboardEvent>();
	

	void Update ()
    {
		
	}
}

[System.Serializable]
public sealed class KeyboardEvent
{
    public KeyCode m_key;
    public UnityEngine.Events.UnityEvent m_event;
}