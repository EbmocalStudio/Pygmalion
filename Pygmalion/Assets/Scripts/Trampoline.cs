using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

    [Range(-1, 1)]
    public float factorX = 1;
    [Range(-1, 1)]
    public float factorY = -1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D body = collision.rigidbody;
        if (body){
            Vector3 vel = body.velocity;
            vel.y = vel.y * factorY;
            vel.x = vel.x * factorX;
            body.velocity = vel;
            Debug.Log(vel);
        }
    }
}
