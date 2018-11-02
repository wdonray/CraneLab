using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkStateMachine : StateMachineBehaviour {

    private AIGuideBehaviour AI;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.gameObject.GetComponent<AIGuideBehaviour>();
        if (AI.m_tyingComplete)
        {
            AI.transform.LookAt(new Vector3(AI.m_startPos.x, AI.transform.position.y, AI.m_startPos.z));
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (AI.m_tyingComplete)
        {
            AI.m_startedTying = false;

            AI.m_agent.isStopped = true;

            AI.transform.LookAt(AI.lookAtCrane);

            AI.m_loadCollected = true;

            SendToAnimator.SendTrigger(AI.gameObject, "Hoist");
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
