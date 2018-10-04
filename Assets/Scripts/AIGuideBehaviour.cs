using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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

    public Vector3 playerPos
    {
        get { return Camera.main.transform.position; }
    }

    public Vector3 cranePos
    {
        get { return m_crane.position; }
    }

    public Vector3 loadPos
    {
        get { return m_load.position; }
    }

    public Vector3 dropZonePos
    {
        get { return m_dropZone.position; }
    }

    public Vector3 lookAtPlayer
    {
        get { return new Vector3(playerPos.x, transform.position.y, playerPos.z); }
    }

    public Vector3 lookAtLoad
    {
        get { return new Vector3(loadPos.x, transform.position.y, loadPos.z); }
    }

    void Awake()
    {
        m_startPos = transform.position;
        m_agent = GetComponent<NavMeshAgent>();
        transform.LookAt(lookAtPlayer);
        SendToAnimator.SendTrigger(gameObject, "Idle");
    }

    void LateUpdate()
    {
        //Look at crane at all times
        //if (m_targetReached == false)
        //{
        //    transform.LookAt(new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z));
        //}

        var loadtoCrane = loadPos - cranePos;
        var loadToPlayer = loadPos - playerPos;

        var droptoCrane = dropZonePos - loadPos;
        var droptoPlayer = dropZonePos - playerPos;

        HoistOrLower(cranePos, loadPos, 1.5f);
        //RaiseLowerBoom(m_cranePos, m_loadPos);
        //Stop();
        //Tie(m_loadPos);

        if (!m_loadCollected)
        {
            //Swing(loadtoCrane, loadToPlayer, 4);
            //RetractExtend(m_cranePos, m_loadPos);
        }
        else
        {
            //Swing(droptoCrane, droptoPlayer, 4);
            //RetractExtend(m_loadPos, m_dropZone.position);
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
        var sourceToPlayer = source - playerPos;
        var targetToPlayer = target - playerPos;

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
            if ((crane - playerPos).magnitude > (target - playerPos).magnitude)
            {
                SendToAnimator.SendTrigger(gameObject, "RaiseBoom");
                return true;
            }
        }
        else
        {
            if ((crane - playerPos).magnitude < (target - playerPos).magnitude)
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
    ///     TODO: Fix, stop is interrupting walk should not be happening on same frame
    ///     //WalkStateBehaviour and TyingUpStateMachine
    /// </summary>
    private void Tie(Vector3 target)
    {
        if (m_tyingComplete == false)
        {
            if (m_targetReached == false)
            {
                // CRANE IN RANGE OF LOAD
                if (Physics.OverlapSphere(target, .5f).Contains(m_crane.GetComponent<Collider>()))
                {
                    //Look at the load
                    transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
                    //Crane reached load
                    m_targetReached = true;

                    //Begin walking to load
                    m_agent.stoppingDistance = 1f;
                    m_agent.SetDestination(target);
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
                if (Physics.OverlapSphere(target, 1f).Contains(transform.GetComponent<Collider>()))
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