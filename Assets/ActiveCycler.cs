using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCycler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_toggleObjects = new List<GameObject>();

    private int _toggleIndex;
    public int toggleIndex
    {
        get { return _toggleIndex; }

        set
        {
            m_toggleObjects[_toggleIndex].SetActive(false);
            _toggleIndex = (value % m_toggleObjects.Count);
            m_toggleObjects [_toggleIndex].SetActive(true);
        }
    }



	void Start ()
    {
        Initialize();
	}
	

	void Update ()
    {
		
	}



    private void Initialize()
    {
        foreach(GameObject go in m_toggleObjects)
        {
            go.SetActive(false);
        }

        m_toggleObjects[0].SetActive(true);
    }

    public void ActivateNext(bool increment)
    {
        toggleIndex += increment ? 1 : -1;
    }

}
