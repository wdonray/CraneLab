using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentLoad : MonoBehaviour
{
    public int Index = 8;
    public float DistanceTrigger = 2.5f;
    public GameObject CurrentParent;
    public GameObject NewParent;
    public HookLoop CurrentHookLoop => GetComponent<HookLoop>();
    public bool UnparentDone;
    public bool CheckBase;

    private void ParentObject(int layer)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit,
            Mathf.Infinity, 1 << layer))
        {
            if (hit.transform.gameObject.layer == layer)
            {
                CurrentParent = hit.transform.gameObject;
                transform.parent.SetParent(CurrentParent.transform);
            }
        }
    }

    private void UnParent(HookLoop hookLoop)
    {
        if (UnparentDone == false)
        {
            if (CheckBase)
            {
                FindObjectOfType<AIGuideBehaviour>().CurrentBase.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
            FindObjectOfType<AIGuideBehaviour>().CurrentBase.parent.SetParent(NewParent.transform);
            hookLoop.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            hookLoop.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            UnparentDone = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentParent == null)
        {
            ParentObject(Index);
        }

        if (CurrentHookLoop.Hooked)
        {
            if (CurrentParent?.transform != NewParent.transform)
            {
                var dist = Vector3.Distance(FindObjectOfType<AIGuideBehaviour>().CurrentBase.transform.position, CurrentHookLoop.transform.position);
                if (dist >= DistanceTrigger)
                {
                    UnParent(CurrentHookLoop);
                }
                //UnParent(CurrentHookLoop);
            }
        }
    }
}
