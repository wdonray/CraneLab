using UnityEngine;
using System.Collections;

public class ActiveStateToggler : MonoBehaviour
{
    public KeyCode toggleKey;
    public GameObject target;

	public void ToggleActive () {
        target.SetActive (!target.activeSelf);
	}

    public void Start()
    {
        target = target == null ? gameObject : target;
    }

    public void Update()
    {
        if (toggleKey == KeyCode.None) return;

        else if (Input.GetKeyDown(toggleKey)) ToggleActive();
    }
}
