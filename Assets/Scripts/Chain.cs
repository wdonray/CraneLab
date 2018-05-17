using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chain : MonoBehaviour
{
    [Range(1, 100), SerializeField]
    int m_chainLength;

    [SerializeField]
    private GameObject m_chainLinkPrefab;

    private Stack<ConfigurableJoint> m_chainLinks;

    private void Start()
    {
        m_chainLinks = new Stack<ConfigurableJoint>();
    }

    private void Update()
    {
        //m_chainLength = (int)Mathf.Abs(Mathf.Sin(Time.time) * 100f);
    }

    private void FixedUpdate()
    {
        if(m_chainLength < m_chainLinks.Count)
        {
            DestroyTopLink();
        }
        else if(m_chainLength > m_chainLinks.Count)
        {
            AddLink();
        }
    }

    [ContextMenu("Add Link")]
    private void AddLink()
    {
        GameObject newLink = Instantiate(m_chainLinkPrefab, transform.position, Quaternion.identity) as GameObject;
        newLink.name = "Chain Top";
        ConfigurableJoint newJoint = newLink.GetComponent<ConfigurableJoint>();

        Rigidbody newRB = newLink.GetComponent<Rigidbody>();
        newRB.useGravity = false;
        newRB.isKinematic = true;

        if (m_chainLinks.Count > 0)
        {
            Rigidbody topRB = m_chainLinks.Peek().GetComponent<Rigidbody>();
            topRB.useGravity = true;
            topRB.isKinematic = false;
            m_chainLinks.Peek().connectedBody = newRB;
            m_chainLinks.Peek().name = "Link: " + m_chainLinks.Count.ToString();
        }

        m_chainLinks.Push(newJoint);
    }

    [ContextMenu("Destroy Link")]
    private void DestroyTopLink()
    {
        if (m_chainLinks.Count < 2) return;

        ConfigurableJoint linkToRemove = m_chainLinks.Pop();

        if (m_chainLinks.Peek().connectedBody != null)
        {
            Rigidbody newTopLink = m_chainLinks.Peek().GetComponent<Rigidbody>();
            newTopLink.useGravity = false;
            newTopLink.isKinematic = true;
            newTopLink.transform.position = linkToRemove.transform.position;
        }

        else
        {
            m_chainLinks.Clear();
        }

        Destroy(linkToRemove.gameObject);
    }
}
