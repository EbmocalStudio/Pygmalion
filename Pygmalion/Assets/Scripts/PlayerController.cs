using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Facing {Left = -1, Neutral, Right};
public enum Movement {Grounded, Aerial, Climbing, Neutral};

public class PlayerController : MonoBehaviour {

	// Ref vers d'autre composant
	public Animator anim;
	public Rigidbody2D body;
	public Collider2D coll;

	// Var pour les deplacements
	public Movement movState = Movement.Grounded;
	public float horizontalSpeed = 5.0f;
	public float stopTreshold = 0.1f;
	private Facing facing = Facing.Neutral;
	public float groundAngle = 30.0f;
	public bool canControlInTheAir = false;
	public float jumpForce = 2.0f;
	public float maxJumpTime = 0.1f;
	// < 0 :: pas dans un saut
	private float timeInJump = -1.0f;
	public LayerMask terrainMask;
	public LayerMask ladderMask;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();
		coll = GetComponent<Collider2D>();
	}

	// Update is called once per frame
	void FixedUpdate () {
			float haxis = Input.GetAxis("Horizontal");
			float vaxis = Input.GetAxisRaw("Vertical");
			switch(movState){
				case Movement.Grounded:
					// mouvement horizontal de base
					horizontalMovement(haxis);
					// test pour saut et grimpage
					if (testLadder(vaxis)){}
					else if (testJump(vaxis)){}
					else if (!testGround()){
						movState = Movement.Aerial;
					}
					break;
				case Movement.Aerial:
					if (canControlInTheAir){
						horizontalMovement(haxis);
					}
					if (!testLadder(vaxis)){
						continueJump(vaxis);
						if (testGround()){
							movState = Movement.Grounded;
						}
					}
					break;
				case Movement.Climbing:
					// TODO
					movState = Movement.Aerial;
					break;
				case Movement.Neutral:
					// Lorsque les controles ne sont plus dans les mains du joueur
					if (testGround()){
						movState = Movement.Grounded;
					}
					break;
			}
	}

	// calcule la direction vers laquelle le personnage regarde
	// float f := vitesse du personnage
	private Facing computeFacing(float f){
		if (f < 0){
			return Facing.Left;
		}
		else if (f == 0){
			return Facing.Neutral;
		}
		else{
			return Facing.Right;
		}
	}

	// gere les mouvements horizontal du personnage
	// float haxis := input horizontal
	private void horizontalMovement(float haxis){
		float speed = horizontalSpeed * haxis;
		Facing nFace = computeFacing(haxis);

		if (nFace == facing && Mathf.Abs(body.velocity.x) <= stopTreshold){
			speed = 0;
		}

		// Pour que ca fonctionne bien, il faut que les colliders aie un truc special
		// flip the sprite if necessary
		if (nFace != Facing.Neutral && nFace != facing){
			Vector3 scale = transform.localScale;
			transform.localScale = new Vector3((int)nFace * Mathf.Abs(scale.x), scale.y, scale.z);
		}

		facing = nFace;
		body.velocity = new Vector2(speed, body.velocity.y);
	}
	// test si le jouer commence a grimper sur une echelle
	// float vaxis := input vertical
	private bool testLadder(float vaxis){
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(ladderMask);
		Collider2D[] collisions = new Collider2D[1];
		if (vaxis != 0 && coll.GetContacts(filter, collisions) > 0){
			// on essai de grimper sur une echelle!!!
			// TODO
			return false;
		}
		return false;
	}
	// test si le joueur commence a sauter
	// float vaxis := input vertical
	private bool testJump(float vaxis){
		// test pour les echelles
		// parce qu'on prefere grimper que sauter
		if (vaxis > 0 && timeInJump < 0){
			timeInJump = 0.0f;
			body.AddForce(new Vector2(0, 3*jumpForce));
			movState = Movement.Aerial;
			return true;
		}
		return false;
	}
	// s'occupe d'allonger ou de terminer le saut si necessaire
	// ne test pas si le joueur est encore dans les air.
	// float vaxis := input vaxis
	private void continueJump(float vaxis){
		if (timeInJump >= 0){
			if (vaxis > 0 && timeInJump < maxJumpTime){
				body.AddForce(new Vector2(0, jumpForce));
				timeInJump += Time.deltaTime;
			} else {
				timeInJump = -1.0f;
			}
		}
	}
	// test si le personnage touche le sol
	// -> bool := true si le joueur touche le sol, sinon false
	private bool testGround(){
		Collider2D[] collision = new Collider2D[1];
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(terrainMask);
		filter.SetNormalAngle(groundAngle, 180-groundAngle);
		return coll.GetContacts(filter, collision) > 0;
	}
}
