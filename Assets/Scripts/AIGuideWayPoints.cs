using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;
using UnityEngine;

public class AIGuideWayPoints : MonoBehaviour
{
    private AIGuideBehaviour AiGuide => GetComponent<AIGuideBehaviour>();
    private GuideHelper GuideHelper => FindObjectOfType<GuideHelper>();
    private AIGuideBehaviour _otheraiGuide = null;
    public List<Transform> Points = new List<Transform>();
    public bool WayPointsActive;

    private void Start()
    {
        WayPointsActive = Points.Count != 0;
        foreach (var rigger in GuideHelper.Riggers)
        {
            if (rigger != AiGuide)
            {
                _otheraiGuide = rigger;
            }
        }
    }

    public void MoveToNextPoint()
    {
        if (AiGuide._complete == false)
        {
            if (Vector3.Distance(transform.position, Points[GuideHelper.Index].transform.position) > .5f)
            {
                if (AiGuide.m_tieOnly)
                {
                    AiGuide.GuideStartPos = Points[GuideHelper.Index].transform.position;
                }
                SendToAnimator.ResetAllTriggers(gameObject);
                SendToAnimator.stop = false;
                AiGuide._guideWalk.RotateTowards(Points[GuideHelper.Index].transform.position, 2);
                AiGuide._guideWalk.WalkTowards(Points[GuideHelper.Index].transform.position, .5f , true);
            }
            else
            {
                AiGuide._guideWalk.RotateTowards(AiGuide.LookAtCrane, 2);
                AiGuide._guideWalk.StopWalking();
                if (AiGuide.m_tieOnly == false)
                {
                    AiGuide.StartCheckHoist();
                }

                if (_otheraiGuide.Agent.isStopped)
                {
                    AiGuide.MovingToWayPoint = false;
                }
            }
        }
    }
}
