using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisCheck : MonoBehaviour
{
    public string m_axis;
    public float m_min, m_max;
    public UnityEngine.Events.UnityEvent m_onComplete;

	// Use this for initialization
	private IEnumerator Start ()
    {
        yield return new WaitUntil(() => Input.GetAxis(m_axis) >= m_max);
        yield return new WaitUntil(() => Input.GetAxis(m_axis) <= m_min);

        m_onComplete.Invoke();
        Destroy(this);
	}

}
