using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIGrabLift : MonoBehaviour
{
    public enum AIGrabLiftState
    {
        Idle,
        Walk,
        StepUp,
        StepDown,
        Fall,
        Dead,
    }

    public Transform m_crane;
    public Vector3 CranePos => m_crane.transform.position;
    public Vector3 LookAtCrane => new Vector3(CranePos.x, transform.position.y, CranePos.z);
    public float StoppingDistance;
    public GameObject Target;
    public bool PickUpZoneReached, OnLift;

    public AIGrabLiftState CurrentState;
    private AIGuideWalk _guideWalk;
    private Transform _oldParent;
    private bool _falling;
    private NavMeshAgent _agent;

    private void Start()
    {
        SetState(AIGrabLiftState.Idle);
        transform.LookAt(LookAtCrane);
        _guideWalk = gameObject.AddComponent<AIGuideWalk>();
        _agent = GetComponent<NavMeshAgent>();
        _guideWalk.Agent = _agent;
        _oldParent = transform.parent;
    }

    private void Update()
    {
        if (Target == null) return;

        //
        var pos = Target.transform.position;
        var dist = Vector3.Distance(transform.position, pos);

        //
        switch (CurrentState)
        {
            case AIGrabLiftState.Idle:
                {
                    _guideWalk.StopWalking();
                    break;
                }
            case AIGrabLiftState.Walk:
                {
                    WalkUp(dist, pos);
                    break;
                }
            case AIGrabLiftState.StepUp:
                {
                    StepUp(pos);
                    break;
                }
            case AIGrabLiftState.StepDown:
                {
                    StepDown();
                    break;
                }
            case AIGrabLiftState.Fall:
                {
                    FallOff();
                    break;
                }
            case AIGrabLiftState.Dead:
                {
                    SendToAnimator.SendTriggerForce(gameObject, "Death");
                    break;
                }
            default:
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    public void SetState(AIGrabLiftState state)
    {
        CurrentState = state;
    }

    /// <summary>
    /// 
    /// </summary>
    public void FallOff()
    {
        OnLift = false;
        SendToAnimator.SendTriggerForce(gameObject, "FallOff");
        _falling = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="pos"></param>
    public void WalkUp(float dist, Vector3 pos)
    {
        if (dist > StoppingDistance)
        {
            _guideWalk.WalkTowards(pos, StoppingDistance, false);
        }
        else
        {
            SendToAnimator.StopPlayack(gameObject);
            SetState(AIGrabLiftState.StepUp);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    public void StepUp(Vector3 pos)
    {
        OnLift = true;
        if (OnLift)
        {
            SendToAnimator.SendTriggerForce(gameObject, "StepUp");
            transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
            transform.SetParent(Target.transform);
            transform.GetComponent<Collider>().isTrigger = true;
            _agent.enabled = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void StepDown()
    {
        if (OnLift)
        {
            SendToAnimator.SendTriggerForce(gameObject, "StepDown");
            transform.SetParent(_oldParent);
            transform.GetComponent<Collider>().isTrigger = false;
            _agent.enabled = true;
            OnLift = false;
        }
        else
        {
            SetState(AIGrabLiftState.Idle);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        if (_falling)
        {
            if (other.gameObject.layer == 8)
            {
                transform.SetParent(other.transform);
                transform.GetComponent<Collider>().isTrigger = false;
                _agent.enabled = true;
                SetState(AIGrabLiftState.Dead);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    public void PickUpZone(Transform target)
    {
        GuideHelper.Index++;
        PickUpZoneReached = true;
        Target = target.gameObject;
        SetState(AIGrabLiftState.Walk);
    }

    /// <summary>
    /// 
    /// </summary>
    public void DropOffZone()
    {
        SetState(AIGrabLiftState.StepDown);
    }
}
