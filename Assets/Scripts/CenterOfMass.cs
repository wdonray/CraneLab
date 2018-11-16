using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public Vector3 Center;
    private Rigidbody _rb;
	void Start ()
	{
	    _rb = GetComponent<Rigidbody>();
	    _rb.centerOfMass = Center;
	}

    void OnDrawGizmos()
    {
        if (_rb != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(_rb.centerOfMass, Vector3.one);
        }
    }
}
