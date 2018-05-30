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


    private Vector3 m_topLinkOffset;


    private void Start()
    {
        m_chainLinks = new Stack<ConfigurableJoint>();
        m_topLinkOffset = Vector3.zero;
    }

    private void Update()
    {
        //m_chainLength = (int)Mathf.Abs(Mathf.Sin(Time.time) * 100f);

        m_topLinkOffset.y += Input.GetAxis("RIGHT_VERTICAL") * Time.deltaTime;
        m_chainLinks.Peek().transform.position = transform.position + m_topLinkOffset;

        if (m_topLinkOffset.y >= m_chainLinkPrefab.transform.localScale.y)
        {
            m_chainLength--;
            m_topLinkOffset = Vector3.zero;
        }
        else if (m_topLinkOffset.y <= -1f)
        {
            m_chainLength++;
            m_topLinkOffset = Vector3.zero;
        }

        m_chainLinks.Peek().transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        if (m_chainLength < m_chainLinks.Count)
        {
            DestroyTopLink();
        }
        else if (m_chainLength > m_chainLinks.Count)
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
            m_chainLinks.Peek().transform.parent = null;
        }

        m_chainLinks.Push(newJoint);
    }

    [ContextMenu("Destroy Link")]
    private void DestroyTopLink()
    {
        if (m_chainLinks.Count < 2) return;

        ConfigurableJoint linkToRemove = m_chainLinks.Pop();
        ConfigurableJoint newTopLink = m_chainLinks.Peek();

        if (newTopLink != null && newTopLink.connectedBody != null)
        {
            Rigidbody newTopRB = newTopLink.GetComponent<Rigidbody>();
            newTopRB.useGravity = false;
            newTopRB.isKinematic = true;
            newTopLink.transform.position = linkToRemove.transform.position;
        }

        else
        {
            StartCoroutine(IDelayDestroyLooseLinks(m_chainLinks.ToArray(), 1f));
            m_chainLinks.Clear();
        }

        Destroy(linkToRemove.gameObject);
    }

    public IEnumerator IDelayDestroyLooseLinks(ConfigurableJoint[] objectsToDestroy, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);

        for(int i = 0; i < objectsToDestroy.Length; i++)
        {
            if (objectsToDestroy[i] == null) Destroy(objectsToDestroy[i]);

            else
            {
                while (objectsToDestroy[i].transform.localScale.magnitude > 0.01f)
                {
                    objectsToDestroy[i].transform.localScale *= 0.5f;
                    yield return null;
                }
                Destroy(objectsToDestroy[i].gameObject);
            }
            yield return null;
        }

        yield return null;
    }
}
