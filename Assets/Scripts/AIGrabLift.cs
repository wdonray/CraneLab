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

    public PersonalLiftTest PersonalLift;
    public Transform m_crane;
    public Vector3 CranePos => m_crane.transform.position;
    public Vector3 LookAtCrane => new Vector3(CranePos.x, transform.position.y, CranePos.z);
    public float StoppingDistance, TargetDistance;
    public GameObject Target;
    public bool OnLift, TyerOn;

    public AIGrabLiftState CurrentState;
    private AIGuideWalk _guideWalk;
    [SerializeField] private Transform _oldParent;
    private bool _falling;
    private NavMeshAgent _agent;
    private bool running = true;

    private void Start()
    {
        SetState(AIGrabLiftState.Idle);
        transform.LookAt(LookAtCrane);
        _guideWalk = gameObject.AddComponent<AIGuideWalk>();
        _agent = GetComponent<NavMeshAgent>();
        _guideWalk.Agent = _agent;
        _oldParent = transform.parent;
        TyerOn = true;
    }

    private void Update()
    {
        if (Target == null) return;

        //Grab target direction math
        var pos = Target.transform.position;
        var dir = transform.position - pos;
        var dist = Vector3.Distance(transform.position, pos + (dir.normalized * TargetDistance));

        //Change States of AI
        switch (CurrentState)
        {
            case AIGrabLiftState.Idle:
                {
                    _guideWalk.StopWalking();
                    break;
                }
            case AIGrabLiftState.Walk:
                {
                    WalkUp(dist, dir, pos);
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
                    Dead();
                    break;
                }
            default:
                break;
        }
    }

    /// <summary>
    ///     Set current state
    /// </summary>
    /// <param name="state"></param>
    public void SetState(AIGrabLiftState state)
    {
        CurrentState = state;
    }

    /// <summary>
    ///     Kill the AI
    /// </summary>
    public void Dead()
    {
        transform.rotation = Quaternion.identity;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        SendToAnimator.SendTriggerForce(gameObject, "Death");
    }

    /// <summary>
    ///     Fall off the lift
    /// </summary>
    public void FallOff()
    {
        OnLift = false;
        transform.SetParent(_oldParent);
        transform.GetComponent<Collider>().isTrigger = false;
        _agent.enabled = true;
        gameObject.AddComponent<Rigidbody>();
        SendToAnimator.SendTriggerForce(gameObject, "FallOff");
        _falling = true;
    }

    /// <summary>
    ///     Walk up and grab the lift
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="dir"></param>
    /// <param name="pos"></param>
    public void WalkUp(float dist, Vector3 dir, Vector3 pos)
    {
        if (dist >= StoppingDistance)
        {
            _guideWalk.WalkTowardsDistance(pos, dir, StoppingDistance, TargetDistance);
        }
        else
        {
            SetState(AIGrabLiftState.StepUp);
        }
    }

    /// <summary>
    ///     Grab the lift
    /// </summary>
    /// <param name="pos"></param>
    public void StepUp(Vector3 pos)
    {
        OnLift = true;
        if (!_falling)
        {
            if (OnLift)
            {
                SendToAnimator.ResetTrigger(gameObject, "Walk");
                SendToAnimator.SendTriggerForce(gameObject, "StepUp");
                StartCoroutine(ParentToTarget());
            }
        }
    }

    /// <summary>
    ///     Only parent object once in update
    /// </summary>
    /// <returns></returns>
    private IEnumerator ParentToTarget()
    {
        while (running)
        {
            PersonalLift.enabled = true;
            transform.SetParent(Target.transform);
            transform.localEulerAngles = new Vector3(0, -175, 0);
            transform.localPosition = new Vector3(0, -1.2f, 0.7f);
            transform.GetComponent<Collider>().isTrigger = true;
            _agent.enabled = false;
            yield return new WaitForEndOfFrame();
            running = false;
        }
    }


    /// <summary>
    ///     Step down off the lift
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
                SetState(AIGrabLiftState.Dead);
            }
        }
    }

    /// <summary>
    ///     Triggered once in pick up zone
    /// </summary>
    /// <param name="target"></param>
    public void PickUpZone(Transform target)
    {
        if (TyerOn)
        {
            TyerOn = false;
            GuideHelper.Index++;
            Target = target.gameObject;
            SetState(AIGrabLiftState.Walk);
        }
    }

    /// <summary>
    ///     Triggered in drop off zone
    /// </summary>
    public void DropOffZone()
    {
        if (TyerOn == false)
        {
            GuideHelper.Index++;
            TyerOn = true;
            SetState(AIGrabLiftState.StepDown);
        }
    }
}
