using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
// ReSharper disable InconsistentNaming

public class AIGuideBehaviour : MonoBehaviour
{
    public Transform m_dropZone;
    public Transform m_load;
    public Transform m_crane;

    [HideInInspector] public Vector3 m_startPos;
    [HideInInspector] public bool m_targetReached;
    [HideInInspector] public NavMeshAgent m_agent;
    [HideInInspector] public bool m_startedTying, m_tyingComplete;

    [SerializeField] private bool m_loadCollected;

    void Start()
    {
        m_startPos = transform.position;
        m_agent = GetComponent<NavMeshAgent>();
        transform.LookAt(new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z));
        SendToAnimator.SendTrigger(gameObject, "Idle");
    }

    public Vector3 m_playerPos
    {
        get { return Camera.main.transform.position; }
    }

    void Update()
    {
        //Look at crane at all times
        //if (m_targetReached == false)
        //{
        //    transform.LookAt(new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z));
        //}

        var loadtoCrane = m_load.position - m_crane.position;
        var loadToPlayer = m_load.position - Camera.main.transform.position;

        var droptoCrane = m_dropZone.position - m_load.position;
        var droptoPlayer = m_dropZone.position - Camera.main.transform.position;

        //HoistOrLower(m_crane.position, m_load.position, 1.5f);
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

        SendToAnimator.SendTrigger(gameObject, angleBetween > angle ? "SwingThatWay" : angleBetween < -angle ? "SwingThisWay" : "Stop");
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

        SendToAnimator.SendTrigger(gameObject, sourceToPlayer.magnitude < targetToPlayer.magnitude ? "RetractBoom" : "ExtendBoom");
        return true;
    }

    /// <summary>
    ///     Raise up and down based on the target
    /// </summary>
    /// <param name="crane"></param>
    /// <param name="target"></param>
    private bool HoistOrLower(Vector3 crane, Vector3 target, float distance)
    {
        var craneToTarget = crane - target;

        if (Mathf.Abs(craneToTarget.y) < distance)
        {
            Stop();
            return false;
        }

        SendToAnimator.SendTrigger(gameObject, (crane.y < target.y) ? "Hoist" : "Lower");
        return true;
    }

    /// <summary>
    ///     when it needs to go up and in, or out and down at the same time
    /// </summary>
    /// <param name="crane"></param>
    /// <param name="target"></param>
    private bool RaiseLowerBoom(Vector3 crane, Vector3 target)
    {
        var m_hoist = (crane.y < target.y);
        if (m_hoist)
        {
            if ((crane - m_playerPos).magnitude > (target - m_playerPos).magnitude)
            {
                SendToAnimator.SendTrigger(gameObject, "RaiseBoom");
                return true;
            }
        }
        else
        {
            if ((crane - m_playerPos).magnitude < (target - m_playerPos).magnitude)
            {
                SendToAnimator.SendTrigger(gameObject, "LowerBoom");
                return true;
            }
        }
        Stop();
        return false;
    }

    /// <summary>
    ///     begin stop animation
    /// </summary>
    private void Stop()
    {
        SendToAnimator.SendTrigger(gameObject, "Stop");
    }

    /// <summary>
    ///     TODO: Fix
    /// </summary>
    private void Tie()
    {
        if (m_tyingComplete == false)
        {
            if (m_targetReached == false)
            {
                // CRANE IN RANGE OF LOAD
                if (Physics.OverlapSphere(m_load.position, .5f).Contains(m_crane.GetComponent<Collider>()))
                {
                    //Look at the load
                    transform.LookAt(new Vector3(m_load.transform.position.x, this.transform.position.y, m_load.transform.position.z));
                    //Crane reached load
                    m_targetReached = true;

                    //Begin walking to load
                    m_agent.stoppingDistance = 1f;
                    m_agent.SetDestination(m_load.transform.position);
                    SendToAnimator.SendTrigger(gameObject, "Walk");

                }
                else
                {
                    // CRANE NOT IN RANGE OF LOAD
                    Stop();
                }
            }

            if (m_startedTying == false)
            {
                //Stop and Tie
                if (Physics.OverlapSphere(m_load.position, 1f).Contains(transform.GetComponent<Collider>()))
                {
                    m_startedTying = true;
                    m_agent.isStopped = true;
                    SendToAnimator.SendTrigger(gameObject, "TyingUp");
                }
            }
        }
    }

    public void Death()
    {
        SendToAnimator.SendTrigger(gameObject, "Death");
    }
}