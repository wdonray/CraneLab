using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NVRButtonUnityAction : MonoBehaviour
{
    public NewtonVR.NVRButton Button;
    public UnityEngine.Events.UnityEvent UnityEvent;

    private void Start()
    {
        if (Button == null)
            Button = GetComponent<NewtonVR.NVRButton>();

        if (Button == null) Destroy(this);
    }

    private void Update()
    {
        if (Button.ButtonDown)
        {
            UnityEvent.Invoke();
        }
    }
}
