using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Mouledoux.Callback;
using Mouledoux.Components;
using Obi;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using ObiRope = Obi.ObiRope;
using Random = UnityEngine.Random;

public class TearTest : MonoBehaviour
{
    public float DistanceToTrigger;
    private Vector3 _startPos;
    private Vector3 _currentPos => transform.position;

    [SerializeField] private List<ObiRope> _ropes = new List<ObiRope>();
    public ObiRope Rope;
    private IEnumerator _coroutine;

    [HideInInspector] public bool _running, _distanceReached;
    public bool _failed, _passed;
    private float _delay;

    void Awake()
    {
        _coroutine = BreakCoRo(2);
        foreach (var obi in transform.parent.GetComponentsInChildren<ObiRope>())
        {
            _ropes.Add(obi);
        }
        Rope = _ropes[Random.Range(0, _ropes.Count)];
        _startPos = transform.position;
    }

    public void StartBreakCoroutine()
    {
        StartCoroutine(_coroutine);
    }

    public void StopBreakCoroutine()
    {
        StopCoroutine(_coroutine);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(CheckDistanceAway);
        _distanceReached = true;
        StartBreakCoroutine();
    }

    private IEnumerator BreakCoRo(int ropeIndex)
    {
        Rope.DistanceConstraints.RemoveFromSolver(null);
        Rope.BendingConstraints.RemoveFromSolver(null);

        Rope.Tear(ropeIndex);

        Rope.BendingConstraints.AddToSolver(this);
        Rope.DistanceConstraints.AddToSolver(this);

        Rope.BendingConstraints.SetActiveConstraints();

        Rope.Solver.UpdateActiveParticles();

        _ropes.Remove(Rope);

        if (_ropes.Count > 0)
        {
            if (!_passed && !_failed)
            {
                Rope = _ropes[Random.Range(0, _ropes.Count)];
                switch (_ropes.Count)
                {
                    case 3:
                    {
                        _delay = 20;
                    }
                        break;
                    case 2:
                    {
                        _delay = 2;
                    }
                        break;
                    case 1:
                    {
                        _delay = 1;
                    }
                        break;
                    default:
                        break;
                }

                yield return new WaitForSeconds(_delay);
                _coroutine = BreakCoRo(2);
                StartBreakCoroutine();
            }
        }
        else if(!_passed)
        {
            _failed = true;
            Debug.Log("You failed!");
            Mediator.instance.NotifySubscribers("EmergancyCallback", new Packet());
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    private bool CheckDistanceAway()
    {
        var dist = Vector3.Distance(_startPos, _currentPos);
        return (dist > DistanceToTrigger);
    }

    private bool CheckIfStopped()
    {
        return transform.GetComponent<Rigidbody>().velocity.magnitude <= 0.1f;
    }

    private IEnumerator PlacedDown()
    {
        _running = true;
        yield return new WaitForSeconds(0.3f);
        float timer = 0;

        while (timer < 3f)
        {
            if (CheckIfStopped()) timer += Time.deltaTime;
            else timer = 0f;
            yield return null;

            if (_failed) yield break;
        }

        if (_failed == false)
        {
            _passed = true;
            Debug.Log("You Passed!");
            Mediator.instance.NotifySubscribers("EmergancyCallback", new Packet());
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.transform.CompareTag("Link") || other.transform.CompareTag("Hook")) return;

        if (_distanceReached)
        {
            if (!_failed && _running == false)
            {
                StartCoroutine(PlacedDown());
            }
        }
    }
}
