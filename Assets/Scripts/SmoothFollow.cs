using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget; // Target to follow
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -10f); // Distance and height offset from the target
    [SerializeField] private float followSpeed = 5f; // Speed at which the camera follows the target
    [SerializeField] private float positionSmoothness = 0.2f; // Smoothness factor for camera position

    private Vector3 desiredPosition;
    private Quaternion desiredRotation;

    private void Update()
    {
        if (followTarget != null)
        {
            // Calculate the desired position based on the follow target's position and the offset
            desiredPosition = followTarget.position + offset;

            // Smoothly move the camera towards the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothness * Time.deltaTime);
        }
    }

    /// <summary>
    /// Sets a new target for the camera to follow.
    /// </summary>
    /// <param name="newFollowTarget">The new target to follow.</param>
    public void SetFollowTarget(Transform newFollowTarget)
    {
        followTarget = newFollowTarget;
    }
}
