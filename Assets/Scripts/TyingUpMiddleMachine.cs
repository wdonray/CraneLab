using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TyingUpMiddleMachine : StateMachineBehaviour {
    private AIGuideBehaviour AI;
    private GuideHelper guideHelper;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.gameObject.GetComponent<AIGuideBehaviour>();
        guideHelper = FindObjectOfType<GuideHelper>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var load = guideHelper.Loads[guideHelper.TestType == TestType.Infinite ? GuideHelper.RandomIndexLoad : GuideHelper.Index];
        if (!AIGuideBehaviour.LoadCollected)
        {
            if (!Physics.OverlapSphere(load.transform.transform.position, 1.3f / 2).Contains(AI.m_hook.GetComponent<Collider>()))
            {
                SendToAnimator.m_oldValue = string.Empty;
                AI.m_startedTying = false;
                AIGuideBehaviour.WalkingToTarget = false;
                AI._guideWalk.StopWalking();
            }
        }
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
