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

    // Use this for initialization
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_anim.SetTrigger("Idle");
    }

    public Vector3 m_playerPos
    {
        get { return Camera.main.transform.position; }
    }

    // Update is called once per frame
    void Update()
    {
        //Look at crane at all times
        transform.LookAt(new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z));
        var loadtoCrane = m_load.position - m_crane.position;
        var loadToPlayer = m_load.position - Camera.main.transform.position;

        var droptoCrane = m_dropZone.position - m_load.position;
        var droptoPlayer = m_dropZone.position - Camera.main.transform.position;

        if (!m_loadCollected)
        {
            Swing(loadtoCrane, loadToPlayer);
            RetractExtend(m_crane.position, m_load.position);
        }
        else
        {
            Swing(droptoCrane, droptoPlayer);
            RetractExtend(m_load.position, m_dropZone.position);
        }
    }

    /// <summary>
    ///     Gets the angle between the crane and the player and sets trigger to the correct direction
    /// </summary>
    /// <param name="toCrane"></param>
    /// <param name="toPlayer"></param>
    private void Swing(Vector3 toCrane, Vector3 toPlayer)
    {
        var angleBetween = Vector3.SignedAngle(new Vector3(toCrane.x, 0, toCrane.z), new Vector3(toPlayer.x, 0, toPlayer.z), new Vector3(0, 1, 0));
        m_anim.SetTrigger(angleBetween > 1 ? "SwingThatWay" : "SwingThisWay");
    }

    /// <summary>
    ///     Source is closer then target retract else extend 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    private void RetractExtend(Vector3 source, Vector3 target)
    {
        source.y = 0;
        target.y = 0;
        m_anim.SetTrigger((source - m_playerPos).magnitude < (target - m_playerPos).magnitude ? "RetractBoom" : "ExtendBoom");
    }
}