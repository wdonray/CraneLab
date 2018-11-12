using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMetrics : MonoBehaviour
{
    public delegate bool BoolChecks();

    public List<BoolChecks> FailChecks;

    public float SpeedLimit = 10, VelocityLimit = 10;

    private Vector3 _storeLastFrame;

    [SerializeField] private Rigidbody LiftRigidBody => FindParentRigidBody<Rigidbody>(gameObject);

    public bool _ranIntoSomething = false;

    private Vector3 StoreStartLift;

    private bool _heightReached = false;

    // Use this for initialization
    private void Awake()
    {
        if (LiftRigidBody == null)
            Destroy(this);

        if (LiftRigidBody != null)
            LiftRigidBody.gameObject.AddComponent<LiftColliderBridge>().Init(this);

        StoreStartLift = LiftRigidBody.transform.position;

        FailChecks = new List<BoolChecks>
        {
            MovedTooFast,
            LeaningTooMuch,
            ChangeVelocityTooFast,
        };
    }

    private IEnumerator Start()
    {
        StartCoroutine(StoreLastFramesVelocity());
        yield break;
    }

    private void Update()
    {
        if (Check(2))
        {
            _heightReached = true;
        }
    }

    /// <summary>
    ///     Checks if the object moved too fast
    /// </summary>
    /// <returns></returns>
    private bool MovedTooFast()
    {
        if (LiftRigidBody == null) return false;
        var speed = LiftRigidBody.velocity.magnitude;
        return speed >= SpeedLimit;
    }

    /// <summary>
    ///     Check if object is leaning too much
    /// </summary>
    /// <returns></returns>
    private bool LeaningTooMuch()
    {
        return Vector3.Angle(transform.up, Vector3.up) > 45;
    }

    /// <summary>
    ///     Checks if velocity has changed too fast
    /// </summary>
    /// <returns></returns>
    private bool ChangeVelocityTooFast()
    {
        if (LiftRigidBody == null) return false;
        var diff = Vector3.Distance(LiftRigidBody.velocity, _storeLastFrame);
        return diff > VelocityLimit;
    }

    /// <summary>
    ///     Store last frames velocity
    /// </summary>
    /// <returns></returns>
    private IEnumerator StoreLastFramesVelocity()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            _storeLastFrame = LiftRigidBody.velocity;
        }
    }

    /// <summary>
    ///     Finds the first parent above this object with a rigidbody
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startObject"></param>
    /// <returns></returns>
    public static T FindParentRigidBody<T>(GameObject startObject) where  T : Component
    {
        T returnObject = null;
        var currentObject = startObject;
        while (returnObject == null)
        {
            if (currentObject == currentObject.transform.root) return null;
            currentObject = currentObject.transform.parent.gameObject;
            returnObject = currentObject.GetComponent<T>();
        }
        return returnObject;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    private bool Check(float height)
    {
        var dist = LiftRigidBody.transform.position.y - StoreStartLift.y;
        return (dist > height);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    public void OnCollisionEnter(Collision other)
    {
        if (_heightReached)
        {
            if (other.transform.CompareTag("Hook") || other.transform.CompareTag("Link")) return;
            _ranIntoSomething = true;
        }
    }
}