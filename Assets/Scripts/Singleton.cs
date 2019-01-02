using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //  Check to see if we're about to be destroyed
    private static bool _applicationIsQuitting;

    private static T _instance;

    private static readonly object Lock = new object();

    /// <summary>
    ///     Access singleton instance through this property
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                //Debug.Log("<color=yellow> [Singleton] Instance " + typeof(T) +
                //          " already destroyed returning null </color>");
                return null;
            }

            lock (Lock)
            {
                if (_instance == null)
                {
                    // Search for exisiting instance
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                    //    Debug.Log("<color=yellow> [Singleton] Something went really wrong " + typeof(T) +
                    //                   " - there should never be more than 1 singleton!" +
                    //                   " Reopenning the scene might fix it. </color>");
                        return _instance;
                    }

                    //  Create a new instance if one does not exist
                    if (_instance == null)
                    {
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = "----- " + typeof(T) + " -----";
                        //  Make instance persistent
                        DontDestroyOnLoad(singletonObject);

                        //Debug.Log("<color=yellow> [Singleton] An instance of " + typeof(T) +
                        //          " is needed in the scene, so '" + singletonObject +
                        //          "' was created with DontDestroyOnLoad.  </color>");
                    }
                    else
                    {
                        //Debug.Log("<color=yellow> [Singleton] Using instance already created: " +
                        //          _instance.gameObject.name + " </color>");
                    }
                }
            }
            return _instance;
        }
    }

    private void OnDestroy()
    {
        _applicationIsQuitting = true;
    }
}