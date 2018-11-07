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
    public float RotationSpeed, TargetDistance = 2f;
    [HideInInspector] public Vector3 GuideStartPos, StoreHookPos;
    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public bool m_startedTying, m_tyingComplete, m_walking, CheckHoistCalled;
    public static bool WalkingToTarget, WalkingtoStartPos, LoadCollected;
    public bool m_tieOnly, m_dead;
    private bool m_swing, m_raiselower, m_hoist, m_inout, startedHoist;
    private GuideHelper _guideHelper;
    private float _height;

    public Vector3 CranePos => m_crane.transform.position;
    public Vector3 HookPos => m_hook.position;
    public Vector3 LoadPos => m_load.position;
    public Vector3 DropZonePos => m_dropZone.position;
    public Vector3 LookAtCrane => new Vector3(CranePos.x, transform.position.y, CranePos.z);
    public Vector3 LookAtLoad => new Vector3(LoadPos.x, transform.position.y, LoadPos.z);
    public Vector3 LookatHook => new Vector3(HookPos.x, transform.position.y, HookPos.z);
    public Vector3 LookAtStart => new Vector3(GuideStartPos.x, transform.position.y, GuideStartPos.z);

    void Awake()
    {
        _height = 2;
        GuideStartPos = transform.position;
        Agent = GetComponent<NavMeshAgent>();
        transform.LookAt(LookAtCrane);
        StoreHookPos = HookPos;
        if (!m_tieOnly)
            StartCheckHoist();
        _guideHelper = FindObjectOfType<GuideHelper>();
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
        var angleBetween = Vector3.SignedAngle(new Vector3(toHook.x, 0, toHook.z).normalized, new Vector3(toPlayer.x, 0, toPlayer.z).normalized, new Vector3(0, 1, 0));
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
        var sourceToPlayer = source - CranePos;
        var targetToPlayer = target - CranePos;
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
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="distance"></param>
    private bool HoistOrLower(Vector3 source, Vector3 target, float distance)
    {
        var hookToTarget = source - target;

        if (Mathf.Abs(hookToTarget.y) < distance)
        {
            if (!m_hoist)
            {
                Stop();
            }

            m_hoist = true;
            return false;
        }

        SendToAnimator.SendTrigger(gameObject, (source.y < target.y) ? "Hoist" : "Lower");
        m_hoist = false;
        return true;
    }

    /// <summary>
    ///     when it needs to go up and in, or out and down at the same time
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    private bool RaiseLowerBoom(Vector3 source, Vector3 target)
    {
        var hookToCrane = source - CranePos;
        var targetToCrane = target - CranePos;
        hookToCrane.y = 0;
        targetToCrane.y = 0;

        var m_hoist = (source.y < target.y - .5f);

        if (m_hoist)
        {
            if ((hookToCrane.magnitude > targetToCrane.magnitude) && (hookToCrane - targetToCrane).magnitude > 1)
            {
                SendToAnimator.SendTrigger(gameObject, "RaiseBoom");
                m_raiselower = true;
                return true;
            }
        }
        else if (source.y > target.y + .5f)
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
    //            Agent.SetDestination(target);
    //            SendToAnimator.SendTrigger(gameObject, "Walk");
    //        }

    //        if (m_startedTying == false)
    //        {
    //            Agent.stoppingDistance = 1f;
    //            if (Physics.OverlapSphere(target, Agent.stoppingDistance)
    //                .Contains(transform.GetComponent<Collider>()))
    //            {
    //                m_startedTying = true;
    //                Agent.isStopped = true;
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
            if (WalkingToTarget)
            {
                Agent.stoppingDistance = .01f;
                //Rotate towards target
                var targetRotation = Quaternion.LookRotation(target - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
                Agent.isStopped = false;
                //Walk to target and stop a distance away
                Agent.SetDestination(target + (dir.normalized * TargetDistance));
                Debug.DrawRay(target + (dir.normalized * TargetDistance), Vector3.up, Color.cyan);
                SendToAnimator.SendTriggerForce(gameObject, "Walk");
            }

            if (LoadCollected)
            {
                var zone = _guideHelper.Zones[GuideHelper.Index];
                var load = _guideHelper.Loads[GuideHelper.Index];

                if (Physics.OverlapBox(target, zone.transform.localScale / 2, zone.transform.rotation).Contains(load.transform.GetChild(2).GetComponent<Collider>()))
                {
                    //Crane in range of target, walk to target
                    WalkingToTarget = true;
                }
            }
            else
            {
                if (Physics.OverlapSphere(target, 1.3f).Contains(m_hook.GetComponent<Collider>()))
                {
                    //Crane in range of target, walk to target
                    WalkingToTarget = true;
                }
            }

            var dist = Vector3.Distance(transform.position, target + (dir.normalized * TargetDistance));
            if (dist <= Agent.stoppingDistance)
            {
                //In Range of Load, start walking towards it and begin tying up
                WalkingToTarget = false;
                SendToAnimator.ResetTrigger(gameObject, "Walk");
                SendToAnimator.SendTrigger(gameObject, "TyingUp");
            }
        }
        else
        {
            Agent.stoppingDistance = .5f;

            if (Vector3.Distance(transform.position, GuideStartPos) > Agent.stoppingDistance)
            {
                if (WalkingtoStartPos)
                {
                    //Rotate towards start pos and walk there
                    var targetRotation = Quaternion.LookRotation(GuideStartPos - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
                    Agent.isStopped = false;
                    Agent.SetDestination(GuideStartPos);
                    SendToAnimator.SendTriggerForce(gameObject, "Walk");
                }
            }
            else
            {
                //Stop walking and idle
                WalkingtoStartPos = false;
                Agent.isStopped = true;
                SendToAnimator.ResetTrigger(gameObject, "Walk");
                SendToAnimator.SendTrigger(gameObject, "Idle");
                m_tyingComplete = false;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var load = _guideHelper.Loads[GuideHelper.Index];
        var zone = _guideHelper.Zones[GuideHelper.Index];
        if (LoadCollected)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(zone.transform.position, zone.transform.localScale / 2);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(load.transform.position, 1.3f);
        }
    }
#endif

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
        var toCrane = (LoadCollected) ? DropZonePos - CranePos : LoadPos - CranePos;
        var hookToCrane = HookPos - CranePos;

        var targetPos = (LoadCollected) ? DropZonePos : LoadPos;
        var source = (LoadCollected) ? LoadPos : HookPos;

        if (m_startedTying == false)
        {
            FaceCrane(0.9f);

            //If you are not tying, guide the crane
            if (!m_tieOnly)
            {
                //If the other AI is walking not tying AI holds up stop
                if (WalkingToTarget || WalkingtoStartPos)
                {
                    Stop();
                }
                else
                {
                    if (CheckHoistCalled == false)
                    {
                        SendToAnimator.ResetTrigger(gameObject, "Stop");
                        if (!Swing(hookToCrane.normalized, toCrane.normalized, (int)swingAngle))
                        {
                            if (!RaiseLowerBoom(source, targetPos))
                            {
                                if (!HoistInOut(source, targetPos, hoistInOutDist))
                                {
                                    if (!HoistOrLower(source, targetPos, hoistLowerDist))
                                    {

                                    }
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
    ///     If you are not facing the crane, rotate and look at the crane
    /// </summary>
    private void FaceCrane(float dist)
    {
        if (!m_walking)
        {
            if (!Agent.hasPath)
            {
                var dir = (CranePos - transform.position).normalized;
                var dotProd = Vector3.Dot(dir, transform.forward);
                if (dotProd < dist)
                {
                    var targetRotation = Quaternion.LookRotation(CranePos - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (RotationSpeed + 2) * Time.deltaTime);
                }
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

    /// <summary>
    ///     Tell them to hoist until certain point
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckHoist()
    {
        CheckHoistCalled = true;
        SendToAnimator.ResetTrigger(gameObject, "Stop");
        SendToAnimator.stop = false;
        SendToAnimator.SendTriggerForce(gameObject, "Hoist");
        yield return new WaitUntil(() => Check(_height));
        CheckHoistCalled = false;
    }

    /// <summary>
    ///     Check if hook has been raised past a value
    /// </summary>
    /// <returns></returns>
    private bool Check(float height)
    {
        var dist = HookPos.y - StoreHookPos.y;
        return (dist > height);
    }

    public void StartCheckHoist()
    {
        StartCoroutine(CheckHoist());
    }
}