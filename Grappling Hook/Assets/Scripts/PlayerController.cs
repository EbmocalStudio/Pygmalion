using UnityEngine;

[RequireComponent(typeof(GrappleController))]
public class PlayerController : MonoBehaviour {
	private CameraController _camera;
	private GrappleController _grapple;
	private float _movementSpeed = 3f;

	private void Awake() {
		_camera = Camera.main.GetComponent<CameraController>();
		_grapple = GetComponent<GrappleController>();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		Vector3 direction = Vector3.zero;

		if(Input.GetKey(KeyCode.W))
			direction += transform.forward;
		if(Input.GetKey(KeyCode.S))
			direction -= transform.forward;
		if(Input.GetKey(KeyCode.A))
			direction -= transform.right;
		if(Input.GetKey(KeyCode.D))
			direction += transform.right;

		direction = direction.normalized;

		transform.Translate(direction * _movementSpeed * Time.deltaTime);

		if(Input.GetMouseButtonDown(1))
			_grapple.FireGrapple(transform.position + _camera.cameraOrigin, _camera.transform.forward);
	}
}