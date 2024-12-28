using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Reference to the player
    public Vector3 offset; // Offset position relative to the player
    public float smoothSpeed = 0.2f; // Smoothness factor for camera movement
    public float rotationSpeed = 3f; // Speed of camera rotation
    public float minYAngle = -15f; // Minimum Y-axis angle (looking down)
    public float maxYAngle = 15f; // Maximum Y-axis angle (looking up)
    public float collisionRadius = 0.5f; // Radius of the SphereCast for collision detection
    public LayerMask collisionLayers; // Layers to detect for collision avoidance
    public float maxDelta = 10f; // Maximum allowed movement per frame for smoothing

    private float currentXRotation = 0f; // Current X-axis rotation (yaw)
    private float currentYRotation = 0f; // Current Y-axis rotation (pitch)

    void Start()
    {
        // Initialize rotation values based on the camera's current rotation
        Vector3 angles = transform.eulerAngles;
        currentXRotation = angles.y;
        currentYRotation = angles.x;
    }

    void LateUpdate()
    {
        // Rotate camera when right mouse button is held
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Update rotation based on mouse movement
            currentXRotation += mouseX * rotationSpeed;
            currentYRotation -= mouseY * rotationSpeed;

            // Clamp the Y-axis rotation
            currentYRotation = Mathf.Clamp(currentYRotation, minYAngle, maxYAngle);
        }

        // Compute the camera's rotation
        Quaternion rotation = Quaternion.Euler(currentYRotation, currentXRotation, 0);

        // Update the player's look direction based on the camera rotation
        UpdatePlayerLookDirection(rotation);

        // Calculate the desired position
        Vector3 desiredPosition = player.position + rotation * offset;

        // Clamp large changes in the desired position
        if (Vector3.Distance(transform.position, desiredPosition) > maxDelta)
        {
            desiredPosition =
                transform.position + (desiredPosition - transform.position).normalized * maxDelta;
        }

        // Smooth the desired position
        Vector3 smoothedDesiredPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed
        );

        // Adjust the smoothed position to avoid collisions
        Vector3 finalPosition = AdjustForCollisions(player.position, smoothedDesiredPosition);

        // Smoothly move the camera to the final position
        transform.position = Vector3.Lerp(transform.position, finalPosition, smoothSpeed);

        // Look at the player
        transform.LookAt(player.position);
    }

    private Vector3 AdjustForCollisions(Vector3 targetPosition, Vector3 desiredPosition)
    {
        // Offset the start position slightly above the player
        Vector3 startPosition = targetPosition + Vector3.up * 0.5f; // Offset by 0.5 units upward
        Vector3 direction = desiredPosition - startPosition;
        float distance = direction.magnitude;

        // Perform a SphereCast to check for obstacles
        if (
            Physics.SphereCast(
                startPosition,
                collisionRadius,
                direction.normalized,
                out RaycastHit hit,
                distance,
                collisionLayers
            )
        )
        {
            // Ignore collisions that are too close to the start position
            if (hit.distance < collisionRadius)
            {
                return desiredPosition;
            }

            // Ignore small colliders based on their bounds size
            if (hit.collider.bounds.size.magnitude < 7f)
            {
                return desiredPosition;
            }
            // Move the camera closer to the hit point to avoid collision
            return hit.point - direction.normalized * collisionRadius;
        }

        return desiredPosition;
    }

    private void UpdatePlayerLookDirection(Quaternion cameraRotation)
    {
        // Get the forward direction of the camera, ignoring the Y-axis
        Vector3 forward = cameraRotation * Vector3.forward;
        forward.y = 0; // Flatten the forward direction to keep the player upright

        if (forward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, smoothSpeed);
        }
    }
}
