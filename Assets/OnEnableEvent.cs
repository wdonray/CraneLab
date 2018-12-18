using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableEvent : MonoBehaviour
{
    [SerializeField]
    private bool selfDestruct;
    public UnityEngine.Events.UnityEvent doOnEnable;

    private void OnEnable()
    {
        doOnEnable.Invoke();
        if (selfDestruct) Destroy(this);
    }
}
