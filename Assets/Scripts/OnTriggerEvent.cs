using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class OnTriggerEvent : MonoBehaviour
{
    public string m_Tag = "";

    [Space]

    [SerializeField]
    private int minToTrigger = 0;
    public UnityEngine.Events.UnityEvent OnEnter;

    [Space]

    [SerializeField]
    private int maxToTrigger = 0;
    public UnityEngine.Events.UnityEvent OnExit;



    private GameObject collisionObject;
    private bool canTrigger = true;
    private int objectsInTrigger = 0;


    void Start ()
    {
        //GetComponent<Collider>().isTrigger = true;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger) return;

        if (m_Tag != "")
            if (!other.CompareTag(m_Tag))
                return;

        collisionObject = other.gameObject;
        objectsInTrigger++;

        if (objectsInTrigger >= minToTrigger)
        {
            OnEnter.Invoke();
            canTrigger = false;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (m_Tag != "")
            if (!other.CompareTag(m_Tag))
                return;

        collisionObject = other.gameObject;
        objectsInTrigger--;

        if (objectsInTrigger <= maxToTrigger)
        {
            OnExit.Invoke();
            canTrigger = true;
        }
    }


    public void NotifySubscribers(string subscription)
    {
        Mouledoux.Components.Mediator.instance.NotifySubscribers(subscription, new Mouledoux.Callback.Packet());
    }


    public void SetParent(bool parent)
    {
        if (parent)
            collisionObject.transform.parent = transform;
        else
            collisionObject.transform.parent = null;

        print(collisionObject.name);
    }
}
