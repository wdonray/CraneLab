﻿using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Obi;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using ObiRope = Obi.ObiRope;
using Random = UnityEngine.Random;

public class TearTest : MonoBehaviour
{
    public float Minimum, Maximum;
    public float TimePicked;
    public float DistanceToTrigger;
    private Vector3 _startPos, _currentPos;
    [SerializeField] private List<ObiRope> _ropes = new List<ObiRope>();
    public ObiRope Rope;
    private IEnumerator _coroutine;

    private bool _running;
    public static bool _failed, _passed;
    private float minTemp, maxTemp;
    // Use this for initialization
    void Awake()
    {
        _coroutine = StartBreakEvent(Minimum, Maximum, () => Break(2));
        foreach (var obi in GetComponentsInChildren<ObiRope>())
        {
            _ropes.Add(obi);
        }
        Rope = _ropes[Random.Range(0, _ropes.Count)];
        _startPos = transform.position;
        StartBreakCoroutine();
    }

    void Update()
    {
        _currentPos = transform.position;
    }

    public void StartBreakCoroutine()
    {
        StartCoroutine(_coroutine);
    }

    public void StopBreakCoroutine()
    {
        StopCoroutine(_coroutine);
    }

    public IEnumerator StartBreakEvent(float minimum, float maximum, UnityAction action)
    {
        TimePicked = Random.Range(minimum, maximum);
        yield return new WaitForSeconds(TimePicked);
        Debug.Log("Time reached for " + transform.name);
        yield return new WaitUntil(CheckDistanceAway);
        if (!_failed && _running == false)
        {
            StartCoroutine(PlacedDown());
        }
        Debug.Log("Distance reached for " + transform.name);
        action.Invoke();
        Debug.Log(transform.name + ", " + Rope + " broke!");
    }

    private void Break(int ropeIndex)
    {
        if (_passed == false)
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
                Rope = _ropes[Random.Range(0, _ropes.Count)];
                switch (_ropes.Count)
                {
                    case 3:
                        {
                            minTemp = Minimum / 3.5f;
                            maxTemp = Maximum / 4;
                        }
                        break;
                    default:
                        {
                            minTemp /= 3.5f;
                            maxTemp /= 4;
                        }
                        break;
                }

                _coroutine = StartBreakEvent(minTemp, maxTemp, () => Break(2));
                StartBreakCoroutine();
            }
            else
            {
                _failed = true;
                Debug.Log("You failed!");
            }
        }
    }

    void OnDestroy()
    {
        StopBreakCoroutine();
    }

    private bool CheckDistanceAway()
    {
        var dist = Vector3.Distance(_startPos, _currentPos);
        return (dist > DistanceToTrigger);
    }

    private bool CheckIfStopped()
    {
        return transform.GetChild(2).GetComponent<Rigidbody>().velocity.magnitude <= 0.1f;
    }

    private IEnumerator PlacedDown()
    {
        _running = true;
        yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(CheckIfStopped);
        if (_failed == false)
        {
            _passed = true;
            Debug.Log("You Passed!");
        }
    }
}
