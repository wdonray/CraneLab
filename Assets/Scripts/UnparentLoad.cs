using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentLoad : MonoBehaviour
{
    public int Index = 8;
    public GameObject CurrentParent;
    public GameObject NewParent;
    public HookLoop CurrentHookLoop => GetComponent<HookLoop>();
    private bool unparentDone;
    public bool CheckBase;

    void ParentObject(int layer)
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
                if (unparentDone == false)
                {
                    if (CheckBase)
                    {
                        CurrentHookLoop.transform.parent.GetChild(2).GetComponent<Rigidbody>().constraints =
                            RigidbodyConstraints.None;
                    }

                    transform.parent.SetParent(NewParent.transform);
                    CurrentHookLoop.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    CurrentHookLoop.GetComponent<Rigidbody>().constraints =
                        RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    unparentDone = true;
                }
            }
        }
    }
}
