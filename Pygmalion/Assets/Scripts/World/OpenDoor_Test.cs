using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor_Test : MonoBehaviour {

	void OnItemDeliverySucess(){
		gameObject.SetActive(false);
	}
}
