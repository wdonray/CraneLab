using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInputManager : MonoBehaviour
{
    #region -----SINGLETON-----
    private static JoystickInputManager _instance;

    public static JoystickInputManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<JoystickInputManager>();

            return _instance;
        }
    }

    private void Awake()
    {
        if (instance != this) Destroy(this);

        DontDestroyOnLoad(instance);
    }
    #endregion

    public List<JoystickInput> JoystickInputs = new List<JoystickInput>();

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        foreach (JoystickInput input in JoystickInputs)
        {
            input.SendInputMessage();
            
        }
	}
}

[System.Serializable]
public class JoystickInput
{
    public enum LeftRight
    {
        LEFT,
        RIGHT,
    }
    public LeftRight LeftOrRight;

    public enum Axis
    {
        HORIZONTAL,
        VERTICAL,
        ROTATION,
    }
    public Axis InputAxis;
    
    public string axisName
    {
        get
        {
            return LeftOrRight.ToString() + "_" + InputAxis;
        }
    }

    public void SendInputMessage()
    {
        Mouledoux.Components.Mediator.instance.NotifySubscribers(axisName, new Mouledoux.Callback.Packet());
    }
}
