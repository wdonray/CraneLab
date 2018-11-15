using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;
using Mouledoux.Components;
using UnityEngine;

public class TyingUpStateMachine : StateMachineBehaviour
{
    private AIGuideBehaviour AI;
    private GuideHelper guideHelper;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        guideHelper = FindObjectOfType<GuideHelper>();
        AI = animator.gameObject.GetComponent<AIGuideBehaviour>();
        //AI.m_startedTying = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI.m_startedTying = false;
        AI.m_tyingComplete = true;
        if (AIGuideBehaviour.LoadCollected)
        {
            guideHelper.Loads[GuideHelper.Index].GetComponent<HookLoop>().Drop();
            if (GuideHelper.Index < guideHelper.LoadToZone.Count)
            {
                GuideHelper.Index++;
                guideHelper.reached = true;
                if (guideHelper.tearEnabled == false)
                {
                    Mediator.instance.NotifySubscribers(AI.gameObject.GetInstanceID().ToString(), new Packet());
                }
            }
            AIGuideBehaviour.LoadCollected = AIGuideBehaviour.LoadCollected == false;
            AIGuideBehaviour.WalkingtoStartPos = true;
        }
        else
        {
            guideHelper.Loads[GuideHelper.Index].GetComponent<HookLoop>().HookUp(AI.m_hook.GetComponent<Collider>());
            Mediator.instance.NotifySubscribers(guideHelper.Loads[GuideHelper.Index].transform.GetInstanceID().ToString(), new Packet());
        }

        if (GuideHelper.Index < guideHelper.LoadToZone.Count)
        {
            if (guideHelper.Loads[GuideHelper.Index].GetComponent<HingeJoint>() == true)
            {
                AIGuideBehaviour.LoadCollected = AIGuideBehaviour.LoadCollected == false;
                AIGuideBehaviour.WalkingtoStartPos = true;
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
