using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIGuideBehaviour : MonoBehaviour
{
    public Transform m_dropZone;
    public Transform m_load;
    public Transform m_hook;
    public Transform m_crane;

    [HideInInspector] public Vector3 m_startPos;
    [HideInInspector] public bool m_targetReached;
    [HideInInspector] public NavMeshAgent m_agent;
    [HideInInspector] public bool m_startedTying, m_tyingComplete;

    public bool m_loadCollected;
    private bool m_dead;

    public Vector3 cranePos
    {
        get { return m_crane.transform.position; }
    }

    public Vector3 hookPos
    {
        get { return m_hook.position; }
    }

    public Vector3 loadPos
    {
        get { return m_load.position; }
    }

    public Vector3 dropZonePos
    {
        get { return m_dropZone.position; }
    }

    public Vector3 lookAtCrane
    {
        get { return new Vector3(cranePos.x, transform.position.y, cranePos.z); }
    }

    public Vector3 lookAtLoad
    {
        get { return new Vector3(loadPos.x, transform.position.y, loadPos.z); }
    }

    public Vector3 lookatHook
    {
        get { return new Vector3(hookPos.x, transform.position.y, hookPos.z); }
    }

    void Awake()
    {
        m_startPos = transform.position;
        m_agent = GetComponent<NavMeshAgent>();
        transform.LookAt(lookAtCrane);
        SendToAnimator.SendTrigger(gameObject, "Idle");
    }

    void Update()
    {
        if (!m_dead)
        {
            GuideCrane();
        }
    }

    public void SetDropZone(Transform newZone)
    {
        var dropZone = GetCurrentDropZone();
        dropZone = newZone;
    }

    public Transform GetCurrentDropZone()
    {
        return m_dropZone;
    }

    public void SetLoad(Transform newLoad)
    {
        var load = GetCurrentLoad();
        load = newLoad;
    }

    public Transform GetCurrentLoad()
    {
        return m_load;
    }

    public void SetHook(Transform newHook)
    {
        var hook = GetCurrentHook();
        hook = newHook;
    }

    public Transform GetCurrentHook()
    {
        return m_hook;
    }

    public void SetCrane(Transform newCrane)
    {
        var crane = GetCurrentCrane();
        crane = newCrane;
    }

    public Transform GetCurrentCrane()
    {
        return m_crane;
    }

    /// <summary>
    ///     Gets the angle between the crane and the player and sets trigger to the correct direction
    /// </summary>
    /// <param name="toHook"></param>
    /// <param name="toPlayer"></param>
    /// <param name="angle"></param>
    private bool Swing(Vector3 toHook, Vector3 toPlayer, int angle)
    {
        var angleBetween = Vector3.SignedAngle(new Vector3(toHook.x, 0, toHook.z), new Vector3(toPlayer.x, 0, toPlayer.z), new Vector3(0, 1, 0));
        var shouldntMove = angleBetween < angle && angleBetween > -angle;

        SendToAnimator.SendTrigger(gameObject, angleBetween > angle ? "SwingThatWay" : angleBetween < -angle ? "SwingThisWay" : "");
        return !shouldntMove;
    }

    /// <summary>
    ///     Source is closer then target retract else extend 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="distance"></param>
    private bool HoistInOut(Vector3 source, Vector3 target, float distance)
    {
        source.y = 0;
        target.y = 0;
        var sourceToPlayer = source - cranePos;
        var targetToPlayer = target - cranePos;

        var shouldntMove = (sourceToPlayer - targetToPlayer).magnitude < distance;

        if (shouldntMove)
        {
            Stop();
            return false;
        }

        SendToAnimator.SendTrigger(gameObject, sourceToPlayer.magnitude > targetToPlayer.magnitude ? "HoistIn" : "HoistOut");
        return true;
    }

    /// <summary>
    ///     Raise up and down based on the target
    /// </summary>
    /// <param name="hook"></param>
    /// <param name="target"></param>
    /// <param name="distance"></param>
    private bool HoistOrLower(Vector3 hook, Vector3 target, float distance)
    {
        var hookToTarget = hook - target;

        if (Mathf.Abs(hookToTarget.y) < distance)
        {
            Stop();
            return false;
        }

        SendToAnimator.SendTrigger(gameObject, (hook.y < target.y) ? "Hoist" : "Lower");
        return true;
    }

    /// <summary>
    ///     when it needs to go up and in, or out and down at the same time
    /// </summary>
    /// <param name="hook"></param>
    /// <param name="target"></param>
    private bool RaiseLowerBoom(Vector3 hook, Vector3 target)
    {
        var m_hoist = (hook.y < target.y - .5f);
        if (m_hoist)
        {
            if ((hook - cranePos).magnitude > (target - cranePos).magnitude)
            {
                SendToAnimator.SendTrigger(gameObject, "RaiseBoom");
                return true;
            }
        }
        else if (hook.y > target.y + .5f)
        {
            if ((hook - cranePos).magnitude < (target - cranePos).magnitude)
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
                if (Physics.OverlapSphere(target, .5f).Contains(m_hook.GetComponent<Collider>()))
                {
                    //Look at the load
                    transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
                    //Crane reached load
                    m_targetReached = true;

                    //Begin walking to load
                    m_agent.stoppingDistance = .5f;
                    m_agent.SetDestination(target);
                    SendToAnimator.SendTrigger(gameObject, "Walk");
                }
                else
                {
                    // CRANE NOT IN RANGE OF LOAD
                    //Stop();
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
        var a = GetComponent<Animator>();
        var names = new List<string>();

        for (var i = 0; i < a.parameterCount; i++)
        {
            var p = a.GetParameter(i);
            if (p.name != "Death")
            {
                names.Add(p.name);
            }
        }

        foreach (var n in names)
        {
            a.ResetTrigger(n);
        }
        m_dead = true;
        SendToAnimator.SendTrigger(gameObject, "Death");
    }

    private void GuideCrane()
    {
        var targetToCrane = (m_loadCollected) ? dropZonePos - loadPos : loadPos - hookPos;
        var targetToPlayer = (m_loadCollected) ? dropZonePos - cranePos : loadPos - cranePos;
        var targetPos = (m_loadCollected) ? dropZonePos : loadPos;

        if (m_startedTying == false)
        {
            transform.LookAt(lookAtCrane);

            if (!Swing(targetToCrane, targetToPlayer, 6))
            {
                if (!RaiseLowerBoom(hookPos, targetPos))
                {
                    if (!HoistInOut(hookPos, targetPos, 1.5f))
                    {
                        if (!HoistOrLower(hookPos, targetPos, 1.5f))
                        {

                        }
                    }
                }
            }
        }
        Tie(targetPos);
    }
}