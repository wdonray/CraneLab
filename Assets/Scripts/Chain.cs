using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chain : MonoBehaviour
{
    [SerializeField]
    private GameObject m_chainLinkPrefab;

    private Stack<ConfigurableJoint> m_chainLinks;

    private void Start()
    {
        m_chainLinks = new Stack<ConfigurableJoint>();
        m_chainLinks.Push(GetComponent<ConfigurableJoint>());

        for(int i = 0; i < 92; i++)
        {
            AddLink();
        }
    }

    [ContextMenu("Add Link")]
    private void AddLink()
    {
        GameObject newLink = Instantiate(m_chainLinkPrefab, m_chainLinks.Peek().transform.position, Quaternion.identity) as GameObject;
        newLink.name = "Chain Top";
        ConfigurableJoint newJoint = newLink.GetComponent<ConfigurableJoint>();

        Rigidbody newRB = newLink.GetComponent<Rigidbody>();
        Rigidbody topRB = m_chainLinks.Peek().GetComponent<Rigidbody>();

        newRB.useGravity = false;
        newRB.isKinematic = true;

        topRB.useGravity = true;
        topRB.isKinematic = false;

        m_chainLinks.Peek().connectedBody = newRB;
        m_chainLinks.Peek().name = "Link: " + m_chainLinks.Count.ToString();

        m_chainLinks.Push(newJoint);
    }

    [ContextMenu("Remove Link")]
    private void RemoveLink()
    {
        Destroy(m_chainLinks.Pop().gameObject);
    }
}
