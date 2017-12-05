using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullableForce : MonoBehaviour {


    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponentInParent<Rigidbody>();
        if (rb == null)
            Debug.LogError("No Rigid body found");
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (rb)
        {
            if (collision.gameObject.tag == "Bullet")
            {
                Debug.Log("DONT MOVE I FUCKING SET YOUR VELOCITY TO VECTOR.ZERO");
                rb.velocity = Vector3.zero;
            }
        }
    }
 
}
