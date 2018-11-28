using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentLoad : MonoBehaviour
{
    public GameObject CurrentParent;
    public GameObject NewParent;
    public HookLoop CurrentHookLoop => GetComponent<HookLoop>();

    public bool CheckBase;
    // Update is called once per frame
    void Update()
    {
        if (CurrentParent == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit,
                Mathf.Infinity, 1 << 8))
            {
                if (hit.transform.gameObject.layer == 8)
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                    //Debug.Log("Did Hit");
                    CurrentParent = hit.transform.gameObject;
                    transform.parent.SetParent(CurrentParent.transform);
                }
                else
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
                    //Debug.Log("Did not Hit");
                }
            }
            else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit,
                Mathf.Infinity, 1 << 9))
            {
                if (hit.transform.gameObject.layer == 9)
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                    //Debug.Log("Did Hit");
                    CurrentParent = hit.transform.gameObject;
                    transform.parent.SetParent(CurrentParent.transform);
                }
                else
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
                    //Debug.Log("Did not Hit");
                }
            }
        }

        if (CurrentHookLoop.Hooked)
        {
            if (CurrentParent?.transform != NewParent.transform)
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
            }
        }
    }
}
