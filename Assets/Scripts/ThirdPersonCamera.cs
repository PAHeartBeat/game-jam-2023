using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
	public Transform target;                // The target object to follow
	public float smoothSpeed = 0.125f;      // Smoothness of camera movement

	private float initialX;                  // Initial X coordinate of the camera's position
	private float initialY;                  // Initial Y coordinate of the camera's position

	private void Awake() {
		// Save the initial X and Y coordinates of the camera's position
		initialX = transform.position.x;
		initialY = transform.position.y;
	}

	private void LateUpdate() {
		if (target == null) {
			Debug.LogWarning("No target assigned to SimpleCameraFollow!");
			return;
		}

		// Get the target's position
		Vector3 targetPosition = target.position;

		// Calculate the desired position with the saved X and Y coordinates and the target's Z coordinate
		Vector3 desiredPosition = new Vector3(initialX, initialY, targetPosition.z);

		// Smoothly move the camera towards the desired position
		transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
	}
}
