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
        Index = 0;
        foreach (var rigger in FindObjectsOfType<AIGuideBehaviour>())
        {
            Riggers.Add(rigger);
        }

        for (int i = 0; i < Loads.Count; i++)
        {
            LoadToZone.Add(i, new LoadAndZone(Loads[i], Zones[i]));
        }
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
                rigger._complete = true;
                if (rigger.m_tieOnly == false)
                {
                    SendToAnimator.stop = false;
                    SendToAnimator.ResetAllTriggers(rigger.gameObject);
                    SendToAnimator.SendTrigger(rigger.gameObject, "JobComplete");
                }
                else
                {
                    rigger.Agent.isStopped = true;
                    SendToAnimator.SendTrigger(rigger.gameObject, "Idle");
                }
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
