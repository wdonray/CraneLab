using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public static string firstName;
    public static string lastName;
    public static string email;



    public void SetFirstName(string name)
    {
        firstName = name;
    }
    public void SetLastName(string name)
    {
        lastName = name;
    }
    public void SetEmail(string newEmail)
    {
        email = newEmail;
    }

    
    public void SetFirstName(UnityEngine.UI.InputField input)
    {
        SetFirstName(input.text);
    }
    public void SetLastName(UnityEngine.UI.InputField input)
    {
        SetLastName(input.text);
    }
    public void SetEmail(UnityEngine.UI.InputField input)
    {
        SetEmail(input.text);
    }
}
