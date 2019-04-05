using UnityEngine;

public class CameraController : MonoBehaviour {
	public float mouseSensitivity = 1;
	public Vector3 cameraOrigin = new Vector3(0, 1, 0);

	private Transform _player;
	private float _maxDistance = 4;
	private float _pitchSpeed = 100;
	private float _yawSpeed = 100;

	private void Awake() {
		_player = transform.parent;
	}

	private void Update() {
		float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

		transform.position = _player.transform.position + cameraOrigin;

		_player.transform.rotation = Quaternion.AngleAxis(mouseX * _pitchSpeed, _player.up) * _player.transform.rotation;

		transform.rotation = Quaternion.AngleAxis(-mouseY * _yawSpeed, transform.right) * transform.rotation;

		RaycastHit hit;
		Physics.Raycast(transform.position, -transform.forward, out hit, _maxDistance);

		float distanceFromPlayer = _maxDistance;
		if(hit.collider != null)
			distanceFromPlayer = hit.distance;

		transform.Translate(new Vector3(0, 0, -distanceFromPlayer));
	}
}