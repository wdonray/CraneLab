using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public static string firstName;
    public static string lastName;
    public static string email;

    public UnityEngine.Events.UnityEvent onStart;

    public void Start()
    {
        onStart.Invoke();
    }


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

    public void AppendToFile()
    {
        string filePath = Application.dataPath + "/" + "UserInformation";

        string info = Date() + ", ";
        info += System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute.ToString("00") + ", ";


        info += lastName + ", ";
        info += firstName + ", ";
        info += email;

        System.IO.StreamWriter file = System.IO.File.AppendText(filePath);
        file.WriteLine(info);
        file.Close();
    }

    private string Date()
    {
        return System.DateTime.Now.Year.ToString() + "/ " +
            System.DateTime.Now.Month.ToString() + "/ " +
            System.DateTime.Now.Day.ToString();
    }

    public void NameToTextBox(UnityEngine.UI.Text textBox)
    {
        textBox.text = firstName + " " + lastName;
    }

    public void DateToTextBox(UnityEngine.UI.Text textBox)
    {
        textBox.text = Date();
    }
}