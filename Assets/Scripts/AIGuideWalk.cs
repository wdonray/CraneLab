using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIGuideWalk : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent Agent;

    public void RotateTowards(Vector3 target, float rotationSpeed)
    {
        var targetRotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void WalkTowardsDistance(Vector3 target, Vector3 dir, float stoppingDistance, float targetDistance)
    {
        Agent.stoppingDistance = stoppingDistance;
        Agent.isStopped = false;
        Agent.SetDestination(target + (dir.normalized * targetDistance));
        Debug.DrawRay(target + (dir.normalized * targetDistance), Vector3.up, Color.cyan);
        SendToAnimator.SendTriggerForce(gameObject, "Walk");
    }

    public void WalkTowards(Vector3 target, float stoppingDistance)
    {
        Agent.stoppingDistance = stoppingDistance;
        Agent.isStopped = false;
        Agent.SetDestination(target);
        SendToAnimator.SendTriggerForce(gameObject, "Walk");
    }

    public void StopWalking()
    {
        Agent.isStopped = true;
        SendToAnimator.ResetTrigger(gameObject, "Walk");
        SendToAnimator.SendTrigger(gameObject, "Idle");
    }
}
