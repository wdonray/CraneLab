using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WalkStateMachine : StateMachineBehaviour
{
    private GuideHelper guideHelper;
    private AIGuideBehaviour AI;
    private int _lastIndex => GuideHelper.Index - 1;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        guideHelper = FindObjectOfType<GuideHelper>();
        AI = animator.gameObject.GetComponent<AIGuideBehaviour>();
        if (AI != null)
            AI.m_walking = true;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (AI == null) return;
        if (AI._complete == false)
        {
            var load = guideHelper.Loads[guideHelper.TestType == TestType.Infinite ? GuideHelper.RandomIndexLoad : GuideHelper.Index];
            if (AIGuideBehaviour.WalkingToTarget)
            {
                if (!AIGuideBehaviour.LoadCollected)
                {
                    if (!Physics.OverlapSphere(load.transform.transform.position, 1.3f / 2)
                        .Contains(AI.m_hook.GetComponent<Collider>()))
                    {
                        AIGuideBehaviour.WalkingToTarget = false;
                        AI._guideWalk.StopWalking();
                    }
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (AI == null) return;
        AIGuideBehaviour other = null;
        SendToAnimator.m_oldValue = string.Empty;
        if (AI.m_tieOnly)
        {
            foreach (var rigger in guideHelper.Riggers)
            {
                if (rigger != AI)
                    other = rigger;
            }

            if (other != null)
            {
                if (AIGuideBehaviour.LoadCollected)
                {
                    other.StoreHookPos = other.HookPos;
                    if (other.WayPoints != null)
                    {
                        if (other.WayPoints.WayPointsActive == false)
                        {
                            other.StartCheckHoist();
                        }
                    }
                    else
                    {
                        other.StartCheckHoist();
                    }
                }
                else
                {
                    other.StoreHookPos = other.HookPos;
                    other.StartCheckHoist();
                }
            }
        }
        AI.m_walking = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
