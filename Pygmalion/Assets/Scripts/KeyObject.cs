using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyObject : MonoBehaviour {

	// nom de l'objet
	public string id;
	// va etre instantie lorsque l'objet est ramasse
	public GameObject replaceWith;
	// si false, une fois ramasse, ce composant va etre desactive
	// si true, l'objet est detruit
	public bool destroyObj = true;

	void OnTriggerEnter2D(Collider2D coll){
		PlayerInventory inv = coll.GetComponent<PlayerInventory>();
		if (inv){
			inv.addObject(id);
			if (destroyObj)
				GameObject.Destroy(gameObject);
			else
				this.enabled = false;
			if (replaceWith)
				GameObject.Instantiate(replaceWith, transform.position, transform.rotation);
		}
	}

}
