using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour {
	[SerializeField] private Transform followTarget; // Target to follow
	[SerializeField] private Transform lookTarget; // Target to look at
	[SerializeField] private Vector3 offset = new Vector3(0f, 3f, -10f); // Distance and height offset from the target
	[SerializeField] private float followSpeed = 5f; // Speed at which the camera follows the target
	[SerializeField] private float rotationSpeed = 10f; // Speed at which the camera rotates
	[SerializeField] private float positionSmoothness = 0.2f; // Smoothness factor for camera position
	[SerializeField] private float rotationSmoothness = 0.2f; // Smoothness factor for camera rotation

	private Vector3 desiredPosition;
	private Quaternion desiredRotation;

	private void Update() {
		if (followTarget != null) {
			// Calculate the desired position based on the follow target's position and the offset
			desiredPosition = followTarget.position + offset;

			// Smoothly move the camera towards the desired position
			transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothness * Time.deltaTime);

			// Rotate the camera towards the look target
			if (lookTarget != null) {
				Vector3 lookDirection = lookTarget.position - transform.position;
				desiredRotation = Quaternion.LookRotation(lookDirection);
				transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothness * Time.deltaTime);
			} else {
				// Rotate the camera towards the follow target's forward direction
				desiredRotation = Quaternion.LookRotation(followTarget.forward);
				transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothness * Time.deltaTime);
			}
		}
	}

	/// <summary>
	/// Sets a new target for the camera to follow.
	/// </summary>
	/// <param name="newFollowTarget">The new target to follow.</param>
	public void SetFollowTarget(Transform newFollowTarget) {
		followTarget = newFollowTarget;
	}

	/// <summary>
	/// Sets a new target for the camera to look at.
	/// </summary>
	/// <param name="newLookTarget">The new target to look at.</param>
	public void SetLookTarget(Transform newLookTarget) {
		lookTarget = newLookTarget;
	}
}
