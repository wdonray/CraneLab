using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldVerification : MonoBehaviour
{
    [SerializeField]
    private List<InputField> m_fieldsToCheck = new List<InputField>();
    private bool hasValidated = false;

    [Space]

    [SerializeField]
    private UnityEngine.Events.UnityEvent onVerify, offVerify;



    void Start()
    {
        foreach (InputField i in m_fieldsToCheck)
        {
            i.onValueChanged.AddListener(delegate { VerifyFields(); });
        }
    }


    public void VerifyFields()
    {
        foreach (InputField i in m_fieldsToCheck)
        {
            if (i.text == "")
            {
                if (hasValidated) offVerify.Invoke();
                hasValidated = false;
                return;
            }
        }

        hasValidated = true;
        onVerify.Invoke();
    }
}