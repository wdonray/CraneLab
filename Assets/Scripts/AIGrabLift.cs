using System;
using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;
using Mouledoux.Components;
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

    public int TimesToTrigger;
    public PersonalLiftTest PersonalLift;
    public Transform m_crane;
    public Vector3 CranePos => m_crane.transform.position;
    public Vector3 LookAtCrane => new Vector3(CranePos.x, transform.position.y, CranePos.z);
    public float StoppingDistance, TargetDistance;
    [HideInInspector] public GameObject Target;
    [HideInInspector] public bool OnLift, TyerOn;
    public Vector3 TargetPos, TargetRot;
    public AIGrabLiftState CurrentState;
    private AIGuideWalk _guideWalk;
    private Transform _oldParent;
    private bool _falling;
    private NavMeshAgent _agent;
    private bool running = true;
    public bool _completed;
    private bool ready;
    private GuideHelper _guideHelper => FindObjectOfType<GuideHelper>();

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
        if (_completed)
        {
            _guideWalk.StopWalking();
            return;
        }
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
            SendToAnimator.m_oldValueForce = string.Empty;
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
            transform.SetParent(Target.transform);

            transform.GetComponent<Collider>().isTrigger = true;
            while (Vector3.Distance(transform.localPosition, TargetPos) > 0.1f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, TargetPos, Time.deltaTime / 2);
                //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, TargetRot, Time.deltaTime);
                yield return null;
            }
            transform.localPosition = TargetPos;
            transform.localEulerAngles = TargetRot;

            foreach (var rigger in _guideHelper.Riggers)
            {
                if (rigger.m_tieOnly == false)
                {
                    rigger.StoreHookPos = rigger.HookPos;
                    rigger.StartCheckHoist();
                }
            }

            PersonalLift.enabled = true;
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
            PersonalLift.enabled = false;
            OnLift = false;
        }
        else
        {
            SetState(AIGrabLiftState.Idle);
            _completed = true;
        }
    }

    /// <summary>
    ///     If the Ai is falling and has hit something trigger dead state
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        if (_falling)
        {
            if (other.transform.CompareTag("DropZone") || other.transform.CompareTag("Hook")) return;
            SetState(AIGrabLiftState.Dead);
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
            foreach (var rigger in _guideHelper.Riggers)
            {
                if (rigger.m_tieOnly == false)
                {
                    SendToAnimator.m_oldValueForce = string.Empty;
                    rigger.Stop();
                }
            }
            Target = target.gameObject;
            StartCoroutine(CheckForMovement(() => PickUpAction(target)));
        }
    }

    /// <summary>
    ///     Action for picking up
    /// </summary>
    /// <param name="target"></param>
    private void PickUpAction(Transform target)
    {
        if (TimesToTrigger > 0)
        {
            TimesToTrigger--;
            GuideHelper.Index++;
        }

        SetState(AIGrabLiftState.Walk);
    }

    /// <summary>
    ///     Action for dropping off
    /// </summary>
    private void DropOffAction()
    {
        if (TimesToTrigger > 0)
        {
            GuideHelper.Index++;
            Mediator.instance.NotifySubscribers(gameObject.GetInstanceID().ToString(), new Packet());
        }

        PersonalLift.enabled = false;
        SetState(AIGrabLiftState.StepDown);
    }

    /// <summary>
    ///     Triggered in drop off zone
    /// </summary>
    public void DropOffZone()
    {
        if (TyerOn == false)
        {
            //TyerOn = true;
            foreach (var rigger in FindObjectsOfType<AIGuideBehaviour>())
            {
                if (rigger.m_tieOnly == false)
                {
                    SendToAnimator.m_oldValueForce = string.Empty;
                    rigger.Stop();
                }
            }
            StartCoroutine(CheckForMovement(DropOffAction));
        }
    }

    /// <summary>
    ///     Wait for target to come to a stop for a few seconds
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    private IEnumerator CheckForMovement(Action func)
    {
        var timer = 0f;

        while (timer < 3f)
        {
            if (TargetStoppedMoving(Target, .5f))
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0f;
            }
            yield return null;
            if (_completed) yield break;
        }

        func.Invoke();
    }

    /// <summary>
    ///     Check if the target has slowed down to less than or equal to the value 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool TargetStoppedMoving(GameObject target, float value)
    {
        if (_guideHelper.LoadToZone[GuideHelper.Index].Zone.GetComponentInChildren<ZoneOnTrigger>().InZone)
        {
            return target.transform.GetComponent<Rigidbody>().velocity.magnitude <= value;
        }
        return false;
    }
}