using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGrabLift : MonoBehaviour
{
    public Transform m_crane;
    public Vector3 CranePos => m_crane.transform.position;
    public Vector3 LookAtCrane => new Vector3(CranePos.x, transform.position.y, CranePos.z);
    public float TargetDistance, StoppingDistance;

    private AIGuideWalk _guideWalk;
    private PersonalLiftTest PersonalLift => FindObjectOfType<PersonalLiftTest>();
    private Transform _oldParent;
    private bool _onLift;

    private void Start()
    {
        transform.LookAt(LookAtCrane);
        _guideWalk = gameObject.AddComponent<AIGuideWalk>();
        _oldParent = transform.parent;
    }

    /// <summary>
    /// 
    /// </summary>
    public void FallOff()
    {
        SendToAnimator.SendTrigger(gameObject, "FallOff");
        transform.parent.SetParent(_oldParent);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    public void WalkStepUp(GameObject target)
    {
        var pos = target.transform.position;
        if (_onLift == false)
        {
            var dir = transform.position - pos;
            var dist = Vector3.Distance(transform.position, pos + (dir.normalized * TargetDistance));

            if (dist >= StoppingDistance)
            {
                _guideWalk.WalkTowardsDistance(pos, dir, .5f, TargetDistance);
            }
            else
            {
                SendToAnimator.SendTrigger(gameObject, "StepUp");
                transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                transform.parent.SetParent(target.transform);
                _onLift = true;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void LetGo()
    {
        if (_onLift)
        {
            SendToAnimator.SendTrigger(gameObject, "StepDown");
            transform.parent.SetParent(_oldParent);
            _onLift = false;
        }
        else
        {
            _guideWalk.StopWalking();
        }
    }
}
