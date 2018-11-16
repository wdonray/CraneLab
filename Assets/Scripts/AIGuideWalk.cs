using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIGuideWalk : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent Agent;
    private bool testBool;
    /// <summary>
    ///     Rotate towards target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rotationSpeed"></param>
    public void RotateTowards(Vector3 target, float rotationSpeed)
    {
        var targetRotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    ///     Walk towards a target with a set distance away 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dir"></param>
    /// <param name="stoppingDistance"></param>
    /// <param name="targetDistance"></param>
    public void WalkTowardsDistance(Vector3 target, Vector3 dir, float stoppingDistance, float targetDistance)
    {
        Agent.stoppingDistance = stoppingDistance;
        Agent.isStopped = false;
        Agent.SetDestination(target + (dir.normalized * targetDistance));
        Debug.DrawRay(target + (dir.normalized * targetDistance), Vector3.up, Color.cyan);
        SendToAnimator.SendTriggerForce(gameObject, "Walk");
    }

    /// <summary>
    ///     Walk towards a target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="stoppingDistance"></param>
    public void WalkTowards(Vector3 target, float stoppingDistance)
    {
        Agent.stoppingDistance = stoppingDistance;
        Agent.isStopped = false;
        Agent.SetDestination(target);
        SendToAnimator.SendTriggerForce(gameObject, "Walk");
    }

    /// <summary>
    ///     Stop walking entirely
    /// </summary>
    public void StopWalking()
    {
        Agent.isStopped = true;
        SendToAnimator.ResetTrigger(gameObject, "Walk");
        SendToAnimator.SendTrigger(gameObject, "Idle");
    }

    /// <summary>
    ///     Used to move the guide and only the guide 
    /// </summary>
    /// <param name="aiGuide"></param>
    /// <param name="guideStartPos"></param>
    /// <param name="guideWalkPos"></param>
    /// <param name="rotationSpeed"></param>
    public void RiggerGuideWalk(AIGuideBehaviour aiGuide, Vector3 guideStartPos, Transform guideWalkPos, float rotationSpeed)
    {
        if (aiGuide.m_tieOnly) return;
        if (AIGuideBehaviour.GuideWalkToLocation)
        {
            if (AIGuideBehaviour.LoadCollected)
            {
                if (Vector3.Distance(transform.position, guideWalkPos.position) > 1)
                {
                    SendToAnimator.ResetAllTriggers(gameObject);
                    RotateTowards(guideWalkPos.position, rotationSpeed);
                    WalkTowards(guideWalkPos.position, 1f);
                }
                else
                {
                    if (!testBool)
                    {
                        guideWalkPos.position = guideStartPos;
                        Agent.isStopped = true;
                        SendToAnimator.ResetAllTriggers(gameObject);
                        aiGuide.CheckHoistCalled = false;
                        aiGuide.StartCheckHoist();
                        AIGuideBehaviour.GuideWalkToLocation = false;
                        testBool = true;
                    }
                }
            }
        }
        else
        {
            RotateTowards(aiGuide.LookAtCrane, rotationSpeed);
        }
    }
}