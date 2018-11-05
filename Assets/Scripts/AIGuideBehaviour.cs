using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.AI;

public class AIGuideBehaviour : MonoBehaviour
{
    public Transform m_dropZone;
    public Transform m_load;
    public Transform m_hook;
    public Transform m_crane;
    public float RotationSpeed;
    [HideInInspector] public Vector3 m_startPos;
    [HideInInspector] public NavMeshAgent m_agent;
    public float TargetDistance = 2f;
    [HideInInspector]
    public bool m_startedTying, m_tyingComplete, m_walking;
    public static bool walkingToLoad, walkingtoStartPos;
    public bool m_tieOnly, m_dead;
    public static bool m_loadCollected;
    private bool m_swing, m_raiselower, m_hoist, m_inout;


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

    public Vector3 lookAtStart
    {
        get { return new Vector3(m_startPos.x, transform.position.y, m_startPos.z); }
    }

    void Awake()
    {
        m_startPos = transform.position;
        m_agent = GetComponent<NavMeshAgent>();
        transform.LookAt(lookAtCrane);
        if (!m_tieOnly)
        {
            SendToAnimator.SendTrigger(gameObject, "Hoist");
            StartCoroutine(PauseAnimator(3));
        }
    }

    void LateUpdate()
    {
        if (!m_dead)
        {
            GuideCrane(4, 1, 1);
        }


    }

    public void SetDropZone(Transform newZone)
    {
        m_dropZone = newZone;
    }

    public void SetLoad(Transform newLoad)
    {
        m_load = newLoad;
    }

    public void SetHook(Transform newHook)
    {
        m_hook = newHook;
    }

    public void SetCrane(Transform newCrane)
    {
        m_crane = newCrane;
    }

    /// <summary>
    ///     Begin stop animation
    /// </summary>
    private void Stop()
    {
        SendToAnimator.SendTriggerForce(gameObject, "Stop");
        StartCoroutine(PauseAnimator(1));
    }

    /// <summary>
    ///     Gets the angle between the crane and the player and sets trigger to the correct direction
    /// </summary>
    /// <param name="toHook"></param>
    /// <param name="toPlayer"></param>
    /// <param name="angle"></param>
    private bool Swing(Vector3 toHook, Vector3 toPlayer, int angle)
    {
        var angleBetween = Vector3.SignedAngle(new Vector3(toHook.x, 0, toHook.z).normalized,
            new Vector3(toPlayer.x, 0, toPlayer.z).normalized, new Vector3(0, 1, 0));
        var shouldntMove = angleBetween < angle && angleBetween > -angle;

        if (shouldntMove)
        {
            if (!m_swing)
            {
                Stop();
            }

            m_swing = true;
            return false;
        }

        var trigger = (angleBetween < 0) ? "SwingThatWay" : "SwingThisWay";

        SendToAnimator.SendTrigger(gameObject, trigger);
        m_swing = false;
        return true;
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
        sourceToPlayer.y = 0;
        targetToPlayer.y = 0;
        var shouldntMove = (sourceToPlayer - targetToPlayer).magnitude < distance;

        if (shouldntMove)
        {
            if (!m_inout)
            {
                Stop();
            }

            m_inout = true;
            return false;
        }

        SendToAnimator.SendTrigger(gameObject,
            sourceToPlayer.magnitude > targetToPlayer.magnitude ? "HoistIn" : "HoistOut");
        m_inout = false;
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
            if (!m_hoist)
            {
                Stop();
            }

            m_hoist = true;
            return false;
        }

        SendToAnimator.SendTrigger(gameObject, (hook.y < target.y) ? "Hoist" : "Lower");
        m_hoist = false;
        return true;
    }

    /// <summary>
    ///     when it needs to go up and in, or out and down at the same time
    /// </summary>
    /// <param name="hook"></param>
    /// <param name="target"></param>
    private bool RaiseLowerBoom(Vector3 hook, Vector3 target)
    {
        var hookToCrane = hook - cranePos;
        var targetToCrane = target - cranePos;
        hookToCrane.y = 0;
        targetToCrane.y = 0;

        var m_hoist = (hook.y < target.y - .5f);

        if (m_hoist)
        {
            if ((hookToCrane.magnitude > targetToCrane.magnitude) && (hookToCrane - targetToCrane).magnitude > 1)
            {
                SendToAnimator.SendTrigger(gameObject, "RaiseBoom");
                m_raiselower = true;
                return true;
            }
        }
        else if (hook.y > target.y + .5f)
        {
            if ((hookToCrane.magnitude < targetToCrane.magnitude) && (hookToCrane - targetToCrane).magnitude > 1)
            {
                SendToAnimator.SendTrigger(gameObject, "LowerBoom");
                m_raiselower = false;
                return true;
            }
        }

        if (!m_raiselower)
        {
            Stop();
        }

        m_raiselower = true;
        return false;
    }

    //Old tie logic
    //private void Tie(Vector3 target)
    //{
    //    if (m_tyingComplete == false)
    //    {
    //        if (m_targetReached == false)
    //        {
    //            // CRANE IN RANGE OF LOAD
    //            if (Physics.OverlapSphere(target, .5f).Contains(m_hook.GetComponent<Collider>()))
    //            {
    //                transform.LookAt(cranePos);
    //                m_targetReached = true;
    //            }
    //        }

    //        if (m_targetReached)
    //        {
    //            m_agent.SetDestination(target);
    //            SendToAnimator.SendTrigger(gameObject, "Walk");
    //        }

    //        if (m_startedTying == false)
    //        {
    //            m_agent.stoppingDistance = 1f;
    //            if (Physics.OverlapSphere(target, m_agent.stoppingDistance)
    //                .Contains(transform.GetComponent<Collider>()))
    //            {
    //                m_startedTying = true;
    //                m_agent.isStopped = true;
    //                SendToAnimator.SendTrigger(gameObject, "TyingUp");
    //            }
    //        }
    //    }
    //}

    /// <summary>
    ///     If the crane is in range of the target walk over and start the tying animation
    /// </summary>
    /// <param name="target"></param>
    private void Tie(Vector3 target)
    {
        if (m_tyingComplete == false)
        {
            var dir = transform.position - target;
            if (walkingToLoad)
            {
                m_agent.stoppingDistance = .01f;
                //Rotate towards target
                var targetRotation = Quaternion.LookRotation(target - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
                m_agent.isStopped = false;
                //Walk to target and stop a distance away
                m_agent.SetDestination(target + (dir.normalized * TargetDistance));
                Debug.DrawRay(target + (dir.normalized * TargetDistance), Vector3.up, Color.cyan);
                SendToAnimator.SendTriggerForce(gameObject, "Walk");
            }

            if (Physics.OverlapSphere(target, 1).Contains(m_hook.GetComponent<Collider>()))
            {
                //Crane in range of target, walk to target
                walkingToLoad = true;
            }

            var dist = Vector3.Distance(transform.position, target + (dir.normalized * TargetDistance));
            if (dist <= m_agent.stoppingDistance)
            {
                //In Range of Load, stop walking towards it and begin tying up
                walkingToLoad = false;
                SendToAnimator.ResetTrigger(gameObject, "Walk");
                SendToAnimator.SendTrigger(gameObject, "TyingUp");
            }
        }
        else
        {
            m_agent.stoppingDistance = .5f;

            if (Vector3.Distance(transform.position, m_startPos) > m_agent.stoppingDistance)
            {
                if (walkingtoStartPos)
                {
                    //Rotate towards start pos and walk there
                    var targetRotation = Quaternion.LookRotation(m_startPos - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
                    m_agent.isStopped = false;
                    m_agent.SetDestination(m_startPos);
                    SendToAnimator.SendTriggerForce(gameObject, "Walk");
                }
            }
            else
            {
                //Stop walking and idle
                walkingtoStartPos = false;
                m_agent.isStopped = true;
                SendToAnimator.ResetTrigger(gameObject, "Walk");
                SendToAnimator.SendTrigger(gameObject, "Idle");
                GuideHelper.Index++;
                m_tyingComplete = false;
            }
        }
    }

    /// <summary>
    ///     Calls this if the agent is hit
    /// </summary>
    public void Death()
    {
        m_dead = true;
        SendToAnimator.SendTrigger(gameObject, "Death");
    }

    /// <summary>
    ///      Guide and or tie the crane using the functions created above
    /// </summary>
    /// <param name="swingAngle"></param>
    /// <param name="hoistInOutDist"></param>
    /// <param name="hoistLowerDist"></param>
    private void GuideCrane(float swingAngle, float hoistInOutDist, float hoistLowerDist)
    {
        Vector3 toCrane;
        if (m_loadCollected)
            toCrane = dropZonePos - cranePos;
        else
            toCrane = loadPos - cranePos;
        var targetPos = (m_loadCollected) ? dropZonePos : loadPos;
        var hookToCrane = hookPos - cranePos;

        if (m_startedTying == false)
        {
            if (!m_walking)
            {
                if (!m_agent.hasPath)
                {
                    //If you are not facing the crane, rotate and look at the crane
                    var dir = (cranePos - transform.position).normalized;
                    var dotProd = Vector3.Dot(dir, transform.forward);
                    if (dotProd < 0.9f)
                    {
                        var targetRotation = Quaternion.LookRotation(cranePos - transform.position);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (RotationSpeed + 2) * Time.deltaTime);
                    }
                }
            }

            //If you are not tying, guide the crane
            if (!m_tieOnly)
            {
                if (walkingToLoad || walkingtoStartPos)
                {
                    Stop();
                }
                else if (!m_agent.hasPath)
                {
                    SendToAnimator.ResetTrigger(gameObject, "Stop");
                    if (!Swing(hookToCrane.normalized, toCrane.normalized, (int)swingAngle))
                    {
                        if (!RaiseLowerBoom(hookPos, targetPos))
                        {
                            if (!HoistInOut(hookPos, targetPos, hoistInOutDist))
                            {
                                if (!HoistOrLower(hookPos, targetPos, hoistLowerDist))
                                {

                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Else tie if needed
                Tie(targetPos);
            }
        }
    }
    /// <summary>
    ///     Pause the animator for a sec amount of seconds
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    public IEnumerator PauseAnimator(int delay)
    {
        SendToAnimator.stop = true;
        yield return new WaitForSeconds(delay);
        SendToAnimator.stop = false;
    }
}