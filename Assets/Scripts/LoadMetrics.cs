using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadMetrics : MonoBehaviour
{
    public delegate bool BoolChecks();

    private bool _heightReached;
    public bool RanIntoSomething;
    private Vector3 _storeLastFrame;
    public List<BoolChecks> FailChecks;
    public float SpeedLimit = 10, VelocityLimit = 10;
    private Vector3 _storeStartLift;
    [SerializeField] private Rigidbody LiftRigidBody => FindParentRigidBody<Rigidbody>(gameObject);
    private GuideHelper _guideHelper => FindObjectOfType<GuideHelper>();
    public Transform GroundTransform;

    // Use this for initialization
    private void Awake()
    {
        if (LiftRigidBody == null)
        {
            Destroy(this);
        }

        if (LiftRigidBody != null)
        {
            _storeStartLift = LiftRigidBody.transform.position;
            LiftRigidBody.gameObject.AddComponent<LiftColliderBridge>().Init(this);
        }

        FailChecks = new List<BoolChecks>
        {
            MovedTooFast,
            LeaningTooMuch,
            ChangeVelocityTooFast
        };
    }

    private IEnumerator Start()
    {
        StartCoroutine(StoreLastFramesVelocity());
        yield break;
    }

    private void Update()
    {
        if (Check(2)) _heightReached = true;
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
    public static T FindParentRigidBody<T>(GameObject startObject) where T : Component
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
    ///     Check if above certain height
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    private bool Check(float height)
    {
        var dist = LiftRigidBody.transform.position.y - _storeStartLift.y;
        return dist > height;
    }

    /// <summary>
    ///     If crashed into something 
    /// </summary>
    /// <param name="other"></param>
    public void OnCollisionEnter(Collision other)
    {
        if (_heightReached)
        {
            if (other.transform.CompareTag("Hook") || other.transform.CompareTag("Link"))
            {
                return;
            }

            if (_guideHelper.Zones[0].transform.GetComponentsInChildren<Transform>().Any(child => other.transform.CompareTag(child.tag)))
            {
                return;
            }

            if (other.transform == GroundTransform)
            {
                return;
            }

            RanIntoSomething = true;
        }
    }
}