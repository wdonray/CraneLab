using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Mouledoux.Callback;
using Mouledoux.Components;
using UnityEngine;
using UnityEngine.AI;

public class AIGuideBehaviour : MonoBehaviour
{
    public string Target;
    public Transform m_dropZone;
    public Transform m_load;
    public Transform m_hook;
    public Transform m_crane;
    public float RotationSpeed, TargetDistance = 2f;
    [HideInInspector] public Vector3 GuideStartPos, StoreHookPos;
    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public bool m_startedTying, m_tyingComplete, m_walking, CheckHoistCalled, Emergancy;
    public static bool WalkingToTarget, WalkingtoStartPos, LoadCollected;
    public bool m_tieOnly, m_dead, _complete;
    private bool m_swing, m_raiselower, m_hoist, m_inout, startedHoist, _tearTriggered, _tearFailed, _tearPassed;
    private GuideHelper _guideHelper;
    private float _height;
    public Vector3 CranePos => m_crane.transform.position;
    public Vector3 HookPos => m_hook.position;
    public Vector3 LoadPos => m_load.position;
    public Vector3 DropZonePos => m_dropZone.position;
    public Vector3 LookAtCrane => new Vector3(CranePos.x, transform.position.y, CranePos.z);
    public Vector3 LookAtLoad => new Vector3(LoadPos.x, transform.position.y, LoadPos.z);
    public Vector3 LookAtHook => new Vector3(HookPos.x, transform.position.y, HookPos.z);
    public Vector3 LookAtStart => new Vector3(GuideStartPos.x, transform.position.y, GuideStartPos.z);

    void Start()
    {
        ResetStaticVariables();
        _height = 2;
        GuideStartPos = transform.position;
        Agent = GetComponent<NavMeshAgent>();
        transform.LookAt(LookAtCrane);
        StoreHookPos = HookPos;
        _guideHelper = FindObjectOfType<GuideHelper>();
        if (!m_tieOnly)
        {
            StartCheckHoist();
            var id = gameObject.GetInstanceID();
            Mediator.instance.NotifySubscribers(id.ToString(), new Packet());
        }
    }

    void LateUpdate()
    {
        if (!_complete)
        {
            if (!m_dead)
            {
                GuideCrane(4, 1, 1);
            }
        }
    }

    public void ResetStaticVariables()
    {
        WalkingToTarget = false;
        WalkingtoStartPos = false;
        LoadCollected = false;
    }
    /// <summary>
    ///     Sets the drop zone with the passed in argument 
    /// </summary>
    /// <param name="newZone"></param>
    public void SetDropZone(Transform newZone)
    {
        m_dropZone = newZone;
    }

    /// <summary>
    ///     Sets the load with the passed in argument 
    /// </summary>
    /// <param name="newLoad"></param>
    public void SetLoad(Transform newLoad)
    {
        m_load = newLoad;
    }

    /// <summary>
    ///     Sets the hook with the passed in argument 
    /// </summary>
    /// <param name="newHook"></param>
    public void SetHook(Transform newHook)
    {
        m_hook = newHook;
    }

    /// <summary>
    ///     Sets the crane with the passed in argument 
    /// </summary>
    /// <param name="newCrane"></param>
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
    /// <param name="source"></param>
    /// <param name="toPlayer"></param>
    /// <param name="angle"></param>
    private bool Swing(Vector3 source, Vector3 toPlayer, int angle)
    {
        var angleBetween = Vector3.SignedAngle(new Vector3(source.x, 0, source.z).normalized,
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

        var hoist = (source.y < target.y - .5f);

        if (hoist)
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
    /// <param name="tearTriggered"></param>
    /// <param name="passed"></param>
    /// <param name="failed"></param>
    private void Tie(Vector3 target, bool tearTriggered, bool passed, bool failed)
    {
        if (m_dead)
        {
            Death();
        }
        else if (tearTriggered)
        {
            LoadCollected = false;
            if (passed)
            {
                var dir = transform.position - target;
                Agent.stoppingDistance = .5f;
                var targetRotation = Quaternion.LookRotation(target - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
                Agent.isStopped = false;
                //Walk to target and stop a distance away
                Agent.SetDestination(target + (dir.normalized * TargetDistance));
                Debug.DrawRay(target + (dir.normalized * TargetDistance), Vector3.up, Color.cyan);
                SendToAnimator.SendTriggerForce(gameObject, "Walk");
                CheckAndTie(target, dir);
            }
            else
            {
                WalkingtoStartPos = false;
                WalkingToTarget = false;
                Agent.isStopped = true;
                SendToAnimator.SendTrigger(gameObject, "Idle");
            }

            if (failed)
            {
                WalkingtoStartPos = false;
                WalkingToTarget = false;
                Agent.isStopped = true;
                SendToAnimator.SendTrigger(gameObject, "Idle");
            }
        }
        else
        {
            if (m_tyingComplete == false)
            {
                var dir = transform.position - target;
                if (WalkingToTarget)
                {
                    Agent.stoppingDistance = .5f;
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
                    var size = new Vector3(zone.transform.localScale.x, .1f, zone.transform.localScale.z);
                    if (Physics.OverlapBox(target, size, zone.transform.rotation).Contains(load.transform.parent.GetChild(2).GetComponent<Collider>()))
                    {
                        //Crane in range of target, walk to target
                        WalkingToTarget = true;
                        CheckAndTie(target, dir);
                    }
                }
                else
                {
                    var load = _guideHelper.Loads[GuideHelper.Index];
                    if (Physics.OverlapSphere(load.transform.GetChild(0).transform.position, 1.3f / 2).Contains(m_hook.GetComponent<Collider>()))
                    {
                        //Crane in range of target, walk to target
                        WalkingToTarget = true;
                        CheckAndTie(target, dir);
                    }
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
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                            RotationSpeed * Time.deltaTime);
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

                    if (_guideHelper.tearEnabled == false)
                    {
                        if (_guideHelper.reached)
                        {
                            if (GetComponent<TeleportAI>())
                            {
                                Mediator.instance.NotifySubscribers("Teleport", new Packet());
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Check if in range of target and begin tying up
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dir"></param>
    private void CheckAndTie(Vector3 target, Vector3 dir)
    {
        var dist = Vector3.Distance(transform.position, target + (dir.normalized * TargetDistance));
        if (dist <= Agent.stoppingDistance)
        {
            //In Range of Load, start walking towards it and begin tying up
            WalkingToTarget = false;
            SendToAnimator.ResetTrigger(gameObject, "Walk");
            SendToAnimator.SendTrigger(gameObject, "TyingUp");
        }
    }

#if UNITY_EDITOR
    /// <summary>
    ///     Drawing the load and zone collision area
    /// </summary>
    //private void OnDrawGizmos()
    //{
    //    if (_complete == false)
    //    {
    //        if (_guideHelper != null)
    //        {
    //            var load = _guideHelper.Loads[(GuideHelper.Index > 2) ? 2 : GuideHelper.Index];
    //            var zone = _guideHelper.Zones[(GuideHelper.Index > 2) ? 2 : GuideHelper.Index];

    //            var size = new Vector3(zone.transform.localScale.x, .1f,
    //                zone.transform.localScale.z);
    //            Gizmos.color = Color.yellow;
    //            Gizmos.DrawWireCube(zone.transform.position, size);

    //            Gizmos.color = Color.red;
    //            Gizmos.DrawWireSphere(load.transform.GetChild(0).transform.position, 1.3f / 2);
    //        }
    //    }
    //}
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
        var sourceToCrane = (LoadCollected) ? LoadPos - CranePos : HookPos - CranePos;

        var targetPos = (LoadCollected) ? DropZonePos : LoadPos;
        Target = targetPos == DropZonePos ? m_dropZone.name : m_load.transform.parent.name;
        var source = (LoadCollected) ? LoadPos : HookPos;

        if (_guideHelper.tearEnabled)
        {
            if (GuideHelper.Index < _guideHelper.LoadToZone.Count)
            {
                _tearTriggered = _guideHelper.Loads[GuideHelper.Index].transform.parent
                    .GetComponentInChildren<TearTest>()._distanceReached;
                _tearFailed = _guideHelper.Loads[GuideHelper.Index].transform.parent.GetComponentInChildren<TearTest>()
                    ._failed;
                _tearPassed = _guideHelper.Loads[GuideHelper.Index].transform.parent.GetComponentInChildren<TearTest>()
                    ._passed;
            }
        }


        if (m_tieOnly)
        {
            if (m_startedTying == false)
            {
                FaceCrane(0.9f);
                Tie(targetPos, _tearTriggered, _tearPassed, _tearFailed);
            }
        }
        else
        {
            //FaceCrane(0.9f);
            //If the other AI is walking AI holds up stop
            if (WalkingToTarget || WalkingtoStartPos)
            {
                Stop();
            }
            else if (_tearTriggered)
            {
                if (_tearFailed)
                {
                    SendToAnimator.SendTriggerOnce(gameObject, "Failed");
                }
                else if (_tearPassed)
                {
                    GuideHelper.Index = _guideHelper.LoadToZone.Count;
                }
                else
                {
                    if (Emergancy == false)
                    {
                        Emergancy = true;
                        SendToAnimator.SendTrigger(gameObject, "EmergancyStop");
                    }
                }
            }
            else
            {
                //Guide the crane
                if (CheckHoistCalled == false)
                {
                    SendToAnimator.ResetTrigger(gameObject, "Stop");
                    if (!Swing(sourceToCrane.normalized, toCrane.normalized, (int)swingAngle))
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
        if (Emergancy)
            return true;
        return (dist > height);
    }

    public void StartCheckHoist()
    {
        if (!Emergancy)
            StartCoroutine(CheckHoist());
    }
}