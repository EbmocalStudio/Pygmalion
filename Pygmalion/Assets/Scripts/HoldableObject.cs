using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HoldableObject : MonoBehaviour {
    Rigidbody2D body;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {}

    void OnActionPerformed(PlayerController controller)
    {
        Debug.Log("HIHI");
        controller.inventory.pickBigObject(body);

    }
    void OnPickup()
    {
        Debug.Log("You picked me!!!");
    }
    void OnDrop()
    {
        Debug.Log("You droped me!!!");

    }
}
