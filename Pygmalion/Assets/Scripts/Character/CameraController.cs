using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
//	static Vector3 center = new Vector3(0.5f, 0.5f, 0.0f);

	// in world
	public bool useBounds = true;
	public Vector3 minBound;
	public Vector3 maxBound;
    private Vector3 minBoundReal, maxBoundReal;

    //private Vector3 minLimit, maxLimit;

    // in viewPort
    public Vector3 minFreedom = new Vector3(0.5f, 0.2f, 9.0f);
	public Vector3 maxFreedom = new Vector3(0.5f, 0.7f, 11.0f);
    public Vector3 offset;

	public Transform target;

	Camera camera;

	public bool debug = false;

	void Start(){
		camera = GetComponent<Camera>();
        Vector3 camMin = camera.ViewportToWorldPoint(Vector3.zero);
        Vector3 camMax = camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        Vector3 camHalf = (camMax - camMin)/2.0f;
        minBoundReal = minBound + camHalf;
        maxBoundReal = maxBound - camHalf;
    }

	// Update is called once per frame
	void Update () {
		if (target){
            Vector3 worldMinFreedom = camera.ViewportToWorldPoint(minFreedom);
            Vector3 worldMaxFreedom = camera.ViewportToWorldPoint(maxFreedom);
            Vector3 potentialTranslation = Util.getDistanceFromRange(target.position, worldMinFreedom, worldMaxFreedom);
            Vector3 newPos = transform.position + potentialTranslation;
            if (useBounds)
				newPos = Util.Clamp(newPos, minBoundReal, maxBoundReal);
			if (debug){
				Debug.Log("Current Pos: " + transform.position);
				Debug.Log("Potential translation: " + potentialTranslation);
				Debug.Log("New pos: " + newPos);
				debug = false;
			}
			transform.position = newPos + offset;
		}

    }

}
