using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideHelper : MonoBehaviour
{
    public static int Index;
    [HideInInspector] public List<AIGuideBehaviour> Riggers = new List<AIGuideBehaviour>();
    public List<GameObject> Loads = new List<GameObject>();
    public List<GameObject> Zones = new List<GameObject>();
    public Dictionary<int, LoadAndZone> LoadToZone = new Dictionary<int, LoadAndZone>();
    // Use this for initialization
    void Awake()
    {
        foreach (var rigger in FindObjectsOfType<AIGuideBehaviour>())
        {
            Riggers.Add(rigger);
        }
        LoadToZone = new Dictionary<int, LoadAndZone>
        {
            { 0, new LoadAndZone(Loads[0], Zones[0])},
            { 1, new LoadAndZone(Loads[1], Zones[1])},
            { 2, new LoadAndZone(Loads[2], Zones[2])},
        };
    }

    // Update is called once per frame
    void Update()
    {
        ActiveLoadToZone(Index);
    }

    void ActiveLoadToZone(int index)
    {
        if (index < Loads.Count)
        {
            foreach (var rigger in Riggers)
            {
                rigger.SetDropZone(LoadToZone[index].Zone.transform);
                rigger.SetLoad(LoadToZone[index].Load.transform);
            }
        }
        else
        {
            foreach (var rigger in Riggers)
            {
                StartCoroutine(rigger.PauseAnimator(1));
                SendToAnimator.ResetAllTriggers(rigger.gameObject);
                SendToAnimator.SendTrigger(rigger.gameObject, "JobComplete");
            }
        }
    }

    public struct LoadAndZone
    {
        public GameObject Load, Zone;

        public LoadAndZone(GameObject load, GameObject zone) : this()
        {
            Load = load;
            Zone = zone;
        }
    }
}
