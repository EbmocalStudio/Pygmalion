using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FableCheckpoint : MonoBehaviour {

    public FableManager fable;

	// Use this for initialization
	void Start () {
		if (!fable)
            fable = (FableManager)FindObjectOfType(typeof(FableManager));
	}
    public void assign(GameObject textObj){}
}
