using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEventManager : MonoBehaviour
{
    #region -----SINGLETON-----
    private static KeyboardEventManager _instance;

    public static KeyboardEventManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<KeyboardEventManager>();

            return _instance;
        }
    }

    private void Awake()
    {
        if (instance != this) Destroy(this);

        DontDestroyOnLoad(instance);
    }
    #endregion

    [SerializeField]
    private List<KeyboardEvent> m_keyEvents = new List<KeyboardEvent>();


    void Update ()
    {
		foreach(KeyboardEvent key in m_keyEvents)
        {
            if (Input.GetKey(key.m_key) && key.m_hold) key.m_event.Invoke();

            else if (Input.GetKeyDown(key.m_key)) key.m_event.Invoke();
        }
	}
}

[System.Serializable]
public sealed class KeyboardEvent
{
    [SerializeField]
    private string m_name;

    public KeyCode m_key;
    public bool m_hold;
    public UnityEngine.Events.UnityEvent m_event;
}