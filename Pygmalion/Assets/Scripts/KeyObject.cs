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
	// si true, l'objet va etre automatiquement ramasse lorsque le personnage le touche
	// sinon, il doit peser sur le bouton ramasser
	public bool autoPickup = true;

	void OnTriggerEnter2D(Collider2D coll){
		if (autoPickup){
			PlayerInventory inv = coll.GetComponent<PlayerInventory>();
			if (inv){
				inv.addObject(id);
				OnObjectAdded();
			}
		}
	}

	void OnObjectAdded(){
		if (destroyObj)
			GameObject.Destroy(gameObject);
		else
			this.enabled = false;
		if (replaceWith)
			GameObject.Instantiate(replaceWith, transform.position, transform.rotation);
	}

	void OnActionPerformed(PlayerController pc){
		pc.inventory.addObject(id);
		OnObjectAdded();
	}

	public static implicit operator string(KeyObject obj){
		return obj.id;
	}

}
