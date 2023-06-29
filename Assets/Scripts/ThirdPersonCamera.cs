using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;        // The target object to follow
    public Vector3 offset = new Vector3(0f, 5f, -5f);    // Camera offset from the target

    public float smoothSpeed = 0.125f;   // Smoothness of camera movement
    public float rotationSpeed = 5f;      // Speed of camera rotation

    public float cinematicDuration = 5f;    // Duration of the cinematic camera view
    public Transform cinematicTarget;       // Target for the cinematic camera view

    private Vector3 desiredPosition;    // Desired position of the camera

    public bool isCinematicView = true;    // Flag to indicate if it's in cinematic view
    private float cinematicTimer = 0f;      // Timer for the cinematic view

    private void Start()
    {
        // Disable player controls during the cinematic view
        // Add code here to disable player controls if needed
    }

    private void LateUpdate()
    {
        if (isCinematicView)
        {
            // Perform cinematic camera movement
            PerformCinematicView();
        }
        else
        {
            // Follow the target character
            FollowTarget();
            AdjustCameraPosition();
            AdjustCameraRotation();
        }
    }

    private void AdjustCameraRotation()
    {
        // Set the desired rotation
        Quaternion desiredRotation = Quaternion.Euler(45f, 0f, 0f);

        // Smoothly rotate the camera towards the desired rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }


    private void AdjustCameraPosition()
    {
        if (target == null)
            return;

        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, rotationSpeed * Time.deltaTime);
    }

    private void PerformCinematicView()
    {
        if (cinematicTimer >= cinematicDuration)
        {
            // Cinematic view is over, switch to following the target
            isCinematicView = false;

            // Re-enable player controls after the cinematic view
            // Add code here to re-enable player controls if needed
            return;
        }

        // Calculate the desired position based on the cinematic target's position and offset
        desiredPosition = cinematicTarget.position + offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Calculate the look direction based on the cinematic target's position and camera's position
        Vector3 lookDirection = cinematicTarget.position - transform.position;
        lookDirection.y = 0f;    // Lock the rotation along the y-axis

        // Rotate the camera smoothly to face the cinematic target
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        // Increase the timer
        cinematicTimer += Time.deltaTime;
    }

    private void FollowTarget()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to ThirdPersonCamera!");
            return;
        }

        // Calculate the desired position based on the target's position and offset
        desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Calculate the look direction based on the target's position and camera's position
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0f;    // Lock the rotation along the y-axis

        // Rotate the camera smoothly to face the target
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

}
