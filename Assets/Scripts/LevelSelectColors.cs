using UnityEngine;
using UnityEngine.UI;

public class LevelSelectColors : MonoBehaviour {

	void Start ()
	{
	    foreach (var child in GetComponentsInChildren<Image>())
	    {
            if (child == GetComponent<Image>()) continue;
	        child.color = DifficultySettings.Instance.DifficultyColor;
	    }
	}
}
