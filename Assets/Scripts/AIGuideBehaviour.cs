using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
// ReSharper disable InconsistentNaming

public class AIGuideBehaviour : MonoBehaviour
{
    [SerializeField] private Animator m_anim;
    public Transform m_dropZone;
    public Transform m_load;
    public Transform m_crane;
    [HideInInspector] public Vector3 m_startPos;
    [SerializeField] private bool m_loadCollected;
    [HideInInspector] public bool m_targetReached;
    [HideInInspector] public NavMeshAgent m_agent;

    private bool m_startedTying;
    // Use this for initialization
    void Start()
    {
        m_startPos = transform.position;
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
        if (m_targetReached == false)
        {
            transform.LookAt(new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z));
        }

        var loadtoCrane = m_load.position - m_crane.position;
        var loadToPlayer = m_load.position - Camera.main.transform.position;

        var droptoCrane = m_dropZone.position - m_load.position;
        var droptoPlayer = m_dropZone.position - Camera.main.transform.position;

        //HoistOrLower(m_crane.position, m_load.position);
        //RaiseLowerBoom(m_crane.position, m_load.position);
        //Stop();
        Tie();

        if (!m_loadCollected)
        {
            //Swing(loadtoCrane, loadToPlayer, 4);
            //RetractExtend(m_crane.position, m_load.position);
        }
        else
        {
            //Swing(droptoCrane, droptoPlayer, 4);
            //RetractExtend(m_load.position, m_dropZone.position);
        }
    }

    /// <summary>
    ///     Gets the angle between the crane and the player and sets trigger to the correct direction
    /// </summary>
    /// <param name="toCrane"></param>
    /// <param name="toPlayer"></param>
    /// <param name="angle"></param>
    private bool Swing(Vector3 toCrane, Vector3 toPlayer, int angle)
    {
        var angleBetween = Vector3.SignedAngle(new Vector3(toCrane.x, 0, toCrane.z), new Vector3(toPlayer.x, 0, toPlayer.z), new Vector3(0, 1, 0));
        var shouldntMove = angleBetween < angle && angleBetween > -angle;

        m_anim.SetTrigger(angleBetween > angle ? "SwingThatWay" : angleBetween < -angle ? "SwingThisWay" : "Stop");
        return !shouldntMove;
    }

    /// <summary>
    ///     Source is closer then target retract else extend 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    private bool RetractExtend(Vector3 source, Vector3 target)
    {
        source.y = 0;
        target.y = 0;
        var sourceToPlayer = source - m_playerPos;
        var targetToPlayer = target - m_playerPos;

        var shouldntMove = (sourceToPlayer - targetToPlayer).magnitude < 1.5f;

        if (shouldntMove)
        {
            Stop();
            return false;
        }

        m_anim.SetTrigger(sourceToPlayer.magnitude < targetToPlayer.magnitude ? "RetractBoom" : "ExtendBoom");
        return true;
    }

    /// <summary>
    ///     Raise up and down based on the target
    /// </summary>
    /// <param name="crane"></param>
    /// <param name="target"></param>
    private bool HoistOrLower(Vector3 crane, Vector3 target)
    {
        var craneToTarget = crane - target;

        if (Mathf.Abs(craneToTarget.y) < 3)
        {
            Stop();
            return false;
        }

        m_anim.SetTrigger((crane.y < target.y) ? "Hoist" : "Lower");
        return true;
    }

    /// <summary>
    ///     when it needs to go up and in, or out and down at the same time
    /// </summary>
    /// <param name="crane"></param>
    /// <param name="target"></param>
    private bool RaiseLowerBoom(Vector3 crane, Vector3 target)
    {
        //TODO: This is not correct but I am close???
        var m_hoist = (crane.y < target.y);
        if (m_hoist)
        {
            if ((crane - m_playerPos).magnitude > (target - m_playerPos).magnitude)
            {
                m_anim.SetTrigger("RaiseBoom");
                return true;
            }
        }
        else
        {
            if ((crane - m_playerPos).magnitude < (target - m_playerPos).magnitude)
            {
                m_anim.SetTrigger("LowerBoom");
                return true;
            }
        }
        Stop();
        return false;
    }

    private void Stop()
    {
        m_anim.SetTrigger("Stop");
    }

    private void Tie()
    {
        #region oof

        /*
        RaycastHit hit;
        if (Physics.Raycast(m_load.position, Vector3.up, out hit))
        {
            //To see if the crane is right above it and if object is the crane
            var hitToCrane = hit.transform.position - m_crane.transform.position;
            var hitObject = hit.collider.gameObject;

            if (hitToCrane.y < 3 && hitObject == m_crane.transform.gameObject)
            {
                //Attempt to say target is reached look and walk there
                m_targetReached = true;
                m_agent.SetDestination(m_load.transform.position);
                transform.LookAt(new Vector3(m_load.transform.position.x, this.transform.position.y, Camera.main.transform.position.z));

                if (m_agent.hasPath == false || m_agent.remainingDistance <= m_agent.stoppingDistance)
                {
                    //Need To play animation once then walk back to start location
                    m_anim.SetTrigger("TyingUp");

                    if (m_anim.GetCurrentAnimatorStateInfo(0).IsName("TyingUp"))
                    {
                        Debug.Log("Done");
                    }
                }
                else
                {
                    //Need to look at correct point and walk there (the load)
                    m_anim.SetTrigger("Walk");
                }
            }
        }
    }
    */

        #endregion

        if (Physics.OverlapSphere(m_load.position, .5f).Contains(m_crane.GetComponent<Collider>()))
        {
            transform.LookAt(new Vector3(m_load.transform.position.x, this.transform.position.y, m_load.transform.position.z));
            if (m_startedTying == false)
            {
                m_targetReached = true;
                m_agent.stoppingDistance = 1f;
                m_agent.SetDestination(m_load.transform.position);
                m_anim.SetTrigger("Walk");
            }
            
            if (Physics.OverlapSphere(m_load.position, 1f).Contains(transform.GetComponent<Collider>()))
            {
                m_startedTying = true;
                m_anim.SetTrigger("TyingUp");
            }
        }
        else
        {
            Stop();
        }
    }
}