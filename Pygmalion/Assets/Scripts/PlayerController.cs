using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Ref vers d'autre composant
	public Animator anim;
	public Rigidbody2D body;

	// Var pour les deplacements
	public float horizontal_speed = 5.0f;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () {

		// Base
		float haxis = Input.GetAxisRaw("Horizontal");
		Vector3 scale = transform.localScale;
		float scalex = Mathf.Abs(scale.x);
		body.velocity = new Vector2(horizontal_speed * haxis, body.velocity.y);
		// flip the sprite if necessary
		if (haxis < 0){
			scalex = -scalex;
		}
		transform.localScale = new Vector3(scalex, scale.y, scale.z);

	}
}
