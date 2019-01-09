using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrthoParallax : MonoBehaviour {

	public Layer[] layers = new Layer[0];
	private Vector3 previousPos;
	public bool onX = true, onY = true, onZ = true;

	// Use this for initialization
	void Start () {
		previousPos = transform.position;
	}

	// Update is called once per frame
	void Update () {
		Vector3 deltaPos = transform.position - previousPos;
		previousPos = transform.position;
		deltaPos = new Vector3(onX ? deltaPos.x : 0, onY ? deltaPos.y : 0, onZ ? deltaPos.z : 0);

		foreach (Layer layer in layers){
			Vector3 trans = deltaPos * layer.relativeSpeed;
			foreach (Transform obj in layer.objects){
                if (obj)
				    obj.Translate(trans);
			}
		}

	}

	[System.Serializable]
	public struct Layer {
		[Range(-1, 1)]
		public float relativeSpeed;
		public Transform[] objects;
	}

}
