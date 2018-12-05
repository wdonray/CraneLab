using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCycler : MonoBehaviour
{
    public List<GameObject> m_toggleObjects = new List<GameObject>();

    private int _toggleIndex;
    public int toggleIndex
    {
        get { return _toggleIndex; }

        set
        {
            m_toggleObjects[_toggleIndex].SetActive(false);

            if(value < 0)
                _toggleIndex = m_toggleObjects.Count - 1;
            
            else
                _toggleIndex = Mathf.Abs(value % m_toggleObjects.Count);

            m_toggleObjects [_toggleIndex].SetActive(true);
        }
    }



	void Start ()
    {
        Initialize();
	}
	

	void Update ()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //    ActivateNext(false);
	}



    private void Initialize()
    {
        foreach(GameObject go in m_toggleObjects)
        {
            go.SetActive(false);
        }

        m_toggleObjects[0].SetActive(true);
    }
    
    public void ActivateNext(bool increment = true)
    {
        toggleIndex += increment ? 1 : -1;
    }

}
