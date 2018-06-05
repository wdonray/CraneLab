using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneSpike : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Obi.ObiRigidbody obiRB = collision.gameObject.GetComponent<Obi.ObiRigidbody>();

        if (obiRB == null) return;

        Obi.ObiRope rope = GetComponent<Obi.ObiRope>();

        print(rope == null);

        Obi.ObiPinConstraintBatch pinConstraints = rope.PinConstraints.GetFirstBatch();

        Vector3 offset = (collision.transform.position - transform.position);
        pinConstraints.AddConstraint(rope.UsedParticles - 1, obiRB.GetComponent<Obi.ObiCollider>(), offset, 0);
    }
}
