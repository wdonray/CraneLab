using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;
using UnityEngine.Events;
using ObiRope = Obi.ObiRope;

public class TearTest : MonoBehaviour
{
    public float DistanceToTrigger;
    private Vector3 _startPos, _currentPos;
    private List<ObiRope> _ropes = new List<ObiRope>();
    public ObiRope Rope;
    private IEnumerator _coroutine;
    public int TimePicked;
    private bool _running;

    // Use this for initialization
    void Awake()
    {
        _coroutine = StartBreakEvent(30, 120, () => Break(2));
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
        if (_running == false)
        {
            StartCoroutine(_coroutine);
        }
    }

    public void StopBreakCoroutine()
    {
        if (_running == true)
        {
            StopCoroutine(_coroutine);
        }
    }

    public IEnumerator StartBreakEvent(int minimum, int maximum, UnityAction action)
    {
        _running = true;
        TimePicked = Random.Range(minimum, maximum);
        yield return new WaitForSeconds(TimePicked);
        yield return new WaitUntil(Check);
        action.Invoke();
        Debug.Log(transform.name + ", " + Rope + " broke!");
        _running = false;
    }

    private void Break(int ropeIndex)
    {
        Rope.DistanceConstraints.RemoveFromSolver(null);
        Rope.BendingConstraints.RemoveFromSolver(null);

        Rope.Tear(ropeIndex);

        Rope.BendingConstraints.AddToSolver(this);
        Rope.DistanceConstraints.AddToSolver(this);

        Rope.BendingConstraints.SetActiveConstraints();

        Rope.Solver.UpdateActiveParticles();
    }

    void OnDestroy()
    {
        StopBreakCoroutine();
    }

    private bool Check()
    {
        var dist = Vector3.Distance(_startPos, _currentPos);
        return (dist > DistanceToTrigger);
    }
}
