using System.Collections;
using System.Collections.Generic;
using Mouledoux.Callback;
using UnityEngine;

public class AIGuideWayPoints : MonoBehaviour
{
    private AIGuideBehaviour aiGuide => GetComponent<AIGuideBehaviour>();
    public List<Transform> Points = new List<Transform>();

    void MoveToNextPoint()
    {
        var sum = GuideHelper.Index - 1;
        if (aiGuide.m_tieOnly)
        {
            aiGuide.GuideStartPos = Points[sum].transform.localPosition;
        }
        aiGuide._guideWalk.RotateTowards(Points[sum].transform.localPosition, 2);
        aiGuide._guideWalk.WalkTowards(Points[sum].transform.localPosition, 0);
    }
}
