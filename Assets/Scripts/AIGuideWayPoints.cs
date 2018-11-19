using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;
using UnityEngine;

public class AIGuideWayPoints : MonoBehaviour
{
    private AIGuideBehaviour aiGuide => GetComponent<AIGuideBehaviour>();
    public List<Transform> Points = new List<Transform>();
    public bool WayPointsActive;

    private void Start()
    {
        WayPointsActive = Points.Count != 0;
    }

    public void MoveToNextPoint()
    {
        if (aiGuide._complete == false)
        {
            if (Vector3.Distance(transform.position, Points[GuideHelper.Index].transform.position) > 1)
            {
                if (aiGuide.m_tieOnly)
                {
                    aiGuide.GuideStartPos = Points[GuideHelper.Index].transform.position;
                }
                SendToAnimator.ResetAllTriggers(gameObject);
                SendToAnimator.stop = false;
                aiGuide._guideWalk.RotateTowards(Points[GuideHelper.Index].transform.position, 2);
                aiGuide._guideWalk.WalkTowards(Points[GuideHelper.Index].transform.position, 1);
            }
            else
            {
                if (aiGuide.m_tieOnly == false)
                {
                    aiGuide.CheckHoistCalled = false;
                    aiGuide.StartCheckHoist();
                }
                aiGuide._guideWalk.RotateTowards(aiGuide.LookAtCrane, 2);
                aiGuide._guideWalk.StopWalking();
                aiGuide.MovingToWayPoint = false;
            }
        }
    }
}
