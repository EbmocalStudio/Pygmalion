using UnityEngine;

public class GrappleController : MonoBehaviour {
	public float maxGrappleDist = 50;

	private enum GrappleState { Resting, Fired, Hooked }
	private GrappleState _grappleState;
	private ConfigurableJoint _grappleJoint;

	public void FireGrapple(Vector3 origin, Vector3 direction) {
		if(_grappleJoint == null) {
			RaycastHit hit;
			Physics.Raycast(origin, direction, out hit, maxGrappleDist);

			if(hit.collider != null) {
				SoftJointLimit jointLimit = new SoftJointLimit() {
					bounciness = 0,
					limit = 10
				};

				_grappleJoint = new ConfigurableJoint();
				_grappleJoint.connectedBody = new Rigidbody();
				_grappleJoint.anchor = hit.point;
				_grappleJoint.linearLimit = jointLimit;
				_grappleJoint.xMotion = ConfigurableJointMotion.Limited;
				_grappleJoint.yMotion = ConfigurableJointMotion.Limited;
				_grappleJoint.zMotion = ConfigurableJointMotion.Limited;
			}
		}
		else
			_grappleJoint = null;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;

		if(_grappleJoint != null) {
			Vector3 anchor = _grappleJoint.anchor;
			Gizmos.DrawLine(new Vector3(anchor.x - 0.5f, anchor.y, anchor.z), new Vector3(anchor.x + 0.5f, anchor.y, anchor.z));
			Gizmos.DrawLine(new Vector3(anchor.x, anchor.y - 0.5f, anchor.z), new Vector3(anchor.x, anchor.y + 0.5f, anchor.z));
			Gizmos.DrawLine(new Vector3(anchor.x, anchor.y, anchor.z - 0.5f), new Vector3(anchor.x, anchor.y, anchor.z + 0.5f));
		}

		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.forward * 5);

		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(transform.position, transform.right * 5);
	}
}