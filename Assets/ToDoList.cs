using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDoList : MonoBehaviour
{
    public Dictionary<string, int> m_scores = new Dictionary<string, int>();

    [SerializeField]
    private List<ScoreEvent> m_initEvents = new List<ScoreEvent>();

    private Dictionary<string, ScoreEvent> m_events = new Dictionary<string, ScoreEvent>();



	// Use this for initialization
	void Start ()
    {
        InitializeDeictionaries();
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckScoreEvents();
	}



    private int InitializeDeictionaries()
    {
        foreach (ScoreEvent se in m_initEvents)
        {
            m_scores.Add(se.name, 0);
            m_events.Add(se.name, se);
        }

        m_initEvents.Clear();
        return 0;
    }

    private int CheckScoreEvents()
    {
        foreach(KeyValuePair<string, ScoreEvent> se in m_events)
        {
            if (m_scores[se.Key] >= se.Value.score) se.Value.unityEvent.Invoke();
        }

        return 0;
    }
}



[System.Serializable]
public class ScoreEvent
{
    public string name;
    public int score;
    public UnityEngine.Events.UnityEvent unityEvent;
}
