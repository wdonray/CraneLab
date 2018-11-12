using UnityEngine;

using System.IO;
using System.Collections;
using System.Collections.Generic;

public class PlayerInformation : MonoBehaviour
{
    private string firstName;
    private string lastName;
    private string email;

    private List<string> keysUsed = new List<string>();
    private Dictionary<string, string> scenarioScoreData = new Dictionary<string, string>();
    

    public string getFirstName => firstName;
    public string getLastName => lastName;
    public string getEmail => email;


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


    public void NameToTextBox(UnityEngine.UI.Text textBox)
    {
        textBox.text = firstName + " " + lastName;
    }
    public void DateToTextBox(UnityEngine.UI.Text textBox)
    {
        textBox.text = Date();
    }


    private string Date()
    {
        return System.DateTime.Now.Year.ToString() + "/ " +
            System.DateTime.Now.Month.ToString() + "/ " +
            System.DateTime.Now.Day.ToString();
    }


    public void AppendToFile()
    {
        string filePath = Application.dataPath + "/" + "UserInformation.csv";

        string info = Date() + ", ";
        info += System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute.ToString("00") + ", ";


        info += lastName + ", ";
        info += firstName + ", ";
        info += email;

        System.IO.StreamWriter file = System.IO.File.AppendText(filePath);
        file.WriteLine(info);
        file.Close();
    }
    

}