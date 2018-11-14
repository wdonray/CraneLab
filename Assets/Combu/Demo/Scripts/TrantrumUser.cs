using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrantrumUser : Combu.User {

    string _firstName = "";
    string _secondName = "";
    string _userEmail = "";
    string _company = "";

    public string FirstName
    {
        get { return _firstName; }
        set { _firstName = value; customData["firstname"] = _firstName; }
    }

    public string SecondName
    {
        get { return _secondName; }
        set { _secondName = value; customData["secondname"] = _secondName; }
    }

    public string Email
    {
        get { return _userEmail; }
        set { _userEmail = value; customData["email"] = _userEmail; }
    }

    public string Company
    {
        get { return _company; }
        set { _company = value; customData["company"] = _company; }
    }

    public TrantrumUser()
    {
        FirstName = "";
        SecondName = "";
        Email = "";
        Company = "";
    }

    public override void FromHashtable(Hashtable hash)
    {
        // Set User class properties
        base.FromHashtable(hash);

        // Set our own custom properties that we store in customData
        if (customData.ContainsKey("firstname"))
            _firstName = customData["firstname"].ToString();

        if (customData.ContainsKey("secondname"))
            _secondName = customData["secondname"].ToString();

        if (customData.ContainsKey("email"))
            _userEmail = customData["email"].ToString();

        if (customData.ContainsKey("company"))
            _company = customData["company"].ToString();
    }
}
