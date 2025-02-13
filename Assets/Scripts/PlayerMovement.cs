using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : NetworkBehaviour
{
    public float walkSpeed = 4f;
    public float maxRunSpeed = 10f;
    public float acceleration = 10f;
    public float jumpForce = 5f;

    public float currentSpeed;
    private Rigidbody rb;
    public bool isGrounded;
    public LayerMask groundLayer;

    private PlayerInput input;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        currentSpeed = walkSpeed;
    }

    void FixedUpdate()
    {
        // ✅ Ensure only the local player can move their character
        if (!IsOwner)
            return;
        // Adjust speed
        if (input.SprintInput && input.MovementInput.magnitude > 0)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                maxRunSpeed,
                acceleration * Time.deltaTime
            );
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                walkSpeed,
                acceleration * Time.deltaTime
            );
        }

        // Apply movement
        Vector3 movement = input.MovementInput.normalized * currentSpeed * Time.fixedDeltaTime;
        transform.Translate(movement, Space.Self);

        // Ground Check with debug logs
        RaycastHit hit;
        bool wasGrounded = isGrounded; // Store previous state for debugging
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, groundLayer);

        // Jump
        if (input.JumpBuffered && isGrounded)
        {
            Debug.Log("🚀 Jumping!");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            input.JumpBuffered = false;
        }
        // Sync movement across the network
        UpdateMovementServerRpc(transform.position);
    }

    [ServerRpc]
    private void UpdateMovementServerRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
