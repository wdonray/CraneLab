using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtils : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ReloadCurrentScene();
    }

    public void GoToScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);
    }

    public void GoToScene(string name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
    }

    public void ReloadCurrentScene()
    {
        GoToScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}