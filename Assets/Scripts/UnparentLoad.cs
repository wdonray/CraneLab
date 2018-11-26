using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentLoad : MonoBehaviour
{
    public GameObject NewParent;
    public HookLoop CurrentHookLoop => GetComponent<HookLoop>();

    // Update is called once per frame
    void Update()
    {
        if (CurrentHookLoop.Hooked)
        {
            if (transform.parent != NewParent.transform)
            {
                transform.parent.SetParent(NewParent.transform);
                CurrentHookLoop.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                CurrentHookLoop.GetComponent<Rigidbody>().constraints =
                    RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }
}
