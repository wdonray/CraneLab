﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mouledoux.Callback;
using Mouledoux.Components;
using UnityEditor;
using UnityEngine;

public class TyingUpStateMachine : StateMachineBehaviour
{
    private AIGuideBehaviour AI;
    private GuideHelper guideHelper => FindObjectOfType<GuideHelper>();
    private HookLoop hookLoop => (guideHelper.TestType == TestType.Infinite) ? guideHelper.Loads[GuideHelper.RandomIndexLoad].GetComponent<HookLoop>() : guideHelper.Loads[GuideHelper.Index].GetComponent<HookLoop>();

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI = animator.gameObject.GetComponent<AIGuideBehaviour>();
        //AI.m_startedTying = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
    private void RandomIndexSelection()
    {
        if (guideHelper.Zones[GuideHelper.Index].GetComponentInChildren<ZoneOnTrigger>().InZone)
        {
            do
            {
                GuideHelper.Index = UnityEngine.Random.Range(0, guideHelper.Zones.Count);
            } while (guideHelper.Zones[GuideHelper.Index].GetComponentInChildren<ZoneOnTrigger>().InZone);
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (AI.m_startedTying == false) return;
        AI.m_startedTying = false;
        AI.m_tyingComplete = true;

        //Drop
        if (AIGuideBehaviour.LoadCollected)
        {
            hookLoop.Drop();
            #region TestType.Infinite 
            if (guideHelper.TestType == TestType.Infinite)
            {
                RandomIndexSelection();
                GuideHelper.RandomIndexLoad = UnityEngine.Random.Range(0, guideHelper.Loads.Count);

                if (!hookLoop.gameObject.activeInHierarchy)
                {
                    hookLoop.gameObject.SetActive(true);
                    hookLoop.GetComponent<LinkPullTowards>().enabled = true;
                }
            }
            #endregion
            else
            {
                if (GuideHelper.Index < guideHelper.Zones.Count)
                {
                    GuideHelper.Index++;
                    guideHelper.reached = true;
                    Mediator.instance.NotifySubscribers(AI.gameObject.GetInstanceID().ToString(), new Packet());
                }

            }
            AIGuideBehaviour.LoadCollected = AIGuideBehaviour.LoadCollected == false;
            AIGuideBehaviour.WalkingtoStartPos = true;
        }
        //Pick Up
        else
        {
            AIGuideBehaviour other = null;
            hookLoop.HookUp(AI.m_hook.GetComponent<Collider>());

            if (GuideHelper.Index < guideHelper.Zones.Count)
            {
                if (guideHelper.Loads[guideHelper.TestType == TestType.Infinite ? GuideHelper.RandomIndexLoad : GuideHelper.Index].GetComponent<HingeJoint>() == true)
                {
                    AIGuideBehaviour.LoadCollected = AIGuideBehaviour.LoadCollected == false;
                    AIGuideBehaviour.WalkingtoStartPos = true;

                    foreach (var rigger in guideHelper.Riggers)
                    {
                        if (rigger != AI)
                            other = rigger;
                    }

                    if (AI.WayPoints != null && AI.WayPoints.WayPointsActive)
                    {
                        AI.MovingToWayPoint = true;
                        other.MovingToWayPoint = true;
                    }
                }
                else
                {
                    SendToAnimator.m_oldValue = String.Empty;
                    AI.m_tyingComplete = false;
                }
            }
        }
    }
}