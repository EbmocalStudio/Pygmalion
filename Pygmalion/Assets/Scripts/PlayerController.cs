using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Ref vers d'autre composant
	public Animator anim;
	public Rigidbody2D body;
	public Collider2D coll;

	// Var pour les deplacements
	public float horizontalSpeed = 5.0f;
	public bool grounded = false;
	public bool controle = true;
	public float groundAngle = 30.0f;
	public bool canControlInTheAir = false;
	public float jumpForce = 2.0f;
	public float maxJumpTime = 0.1f;
	// < 0 :: pas dans un saut
	private float timeInJump = -1.0f;
	public LayerMask terrainMask;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();
		coll = GetComponent<Collider2D>();
	}

	// Update is called once per frame
	void FixedUpdate () {

		// grounded check
		Collider2D[] collision = new Collider2D[1];
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(terrainMask);
		filter.SetNormalAngle(groundAngle, 180-groundAngle);
		if (coll.GetContacts(filter, collision) > 0){
			grounded = true;
	//		timeInJump = -1.0f;
		}
		else {
			grounded = false;
		}

		// Base
		if (controle){

			// horizontal
			float haxis = Input.GetAxis("Horizontal");
			Vector3 scale = transform.localScale;
			float scalex = Mathf.Abs(scale.x);
			if (grounded || canControlInTheAir){
				body.velocity = new Vector2(horizontalSpeed * haxis, body.velocity.y);
			}
			// flip the sprite if necessary
			if (haxis < 0){
				scalex = -scalex;
			}
			transform.localScale = new Vector3(scalex, scale.y, scale.z);

			// vertical
			float vaxis = Input.GetAxisRaw("Vertical");
			if (vaxis > 0){
				// saut de base
				if (canJump()){
					timeInJump = 0.0f;
					body.AddForce(new Vector2(0, 3*jumpForce));
				}
				// augmente la hauteur du saut si on pese plus longtemps
				else if (timeInJump >= 0.0f && timeInJump < maxJumpTime){
					body.AddForce(new Vector2(0, jumpForce));
					timeInJump += Time.deltaTime;
				}
			}
			else { // remmet dans l'etat "pas en saut"
				timeInJump = -1.0f;
			}

		}
	}

	// Pour si les conditions des sauts changent
	private bool canJump(){
		return grounded && timeInJump < 0.0f;
	}
}
