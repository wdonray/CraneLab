using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIGuideBehaviour : MonoBehaviour
{
    [SerializeField] private Animator m_anim;
    public Transform m_dropZone;
    public Transform m_load;
    public Transform m_crane;
    [SerializeField] private bool m_loadCollected;
    private NavMeshAgent m_agent;

    //TODO: Remove this later
    private GameObject _testingCube;
    public Camera MainCamera;

    // Use this for initialization
    void Start()
    {
        _testingCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_agent = GetComponent<NavMeshAgent>();
        m_anim.SetTrigger("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        m_crane = _testingCube.transform;
        FakeCrane();

        //Look at crane at all times
        transform.LookAt(m_crane);

        if (!m_loadCollected)
        {

        }
        else
        {

        }
    }

    /// <summary>
    ///     Used to simulate a crane
    /// </summary>
    void FakeCrane()
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            float oldY = _testingCube.transform.position.y;
            _testingCube.transform.position.Set(hit.point.x, oldY, hit.point.z);
        }
    }
}
