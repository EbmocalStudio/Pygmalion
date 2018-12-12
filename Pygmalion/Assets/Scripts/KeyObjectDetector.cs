using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Destiny {Delete, Disable, Keep};

[RequireComponent(typeof(Collider2D))]
public class KeyObjectDetector : MonoBehaviour {
	// envoye si la tentative reussie ou echoue
	public const string MESSAGE_DELIVERY = "OnItemDelivery";
	// envoye si la tentative reussie
	public const string MESSAGE_SUCCESS = "OnItemDeliverySucess";
	// envoye si la tentative echoue
	public const string MESSAGE_MISS = "OnItemDeliveryMiss";

	public string objectId;
	public uint numberRequired = 1;
	public bool removeObject = true;
	public Destiny afterDelivery = Destiny.Disable;
	public GameObject[] sendMessageTo;

	void OnTriggerEnter2D(Collider2D other){
		PlayerInventory inv;
		if (inv = other.GetComponent<PlayerInventory>()){
			string msg;
			if (inv.containsSmallObject(objectId, numberRequired)){
				if (removeObject)
					inv.removeSmallObject(objectId, numberRequired);
				msg = MESSAGE_SUCCESS;
				switch (afterDelivery){
					case Destiny.Disable:
						this.enabled = false;
						break;
					case Destiny.Delete:
						GameObject.Destroy(gameObject);
						break;
					case Destiny.Keep:
					default:
						break;
				}
			}
			else
				msg = MESSAGE_MISS;

			foreach (GameObject obj in sendMessageTo){
				obj.SendMessage(MESSAGE_DELIVERY, SendMessageOptions.DontRequireReceiver);
				obj.SendMessage(msg, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

}
