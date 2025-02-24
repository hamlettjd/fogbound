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
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        input = GetComponent<PlayerInput>();
        currentSpeed = walkSpeed;
    }

    void FixedUpdate()
    {
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

        // 🏃‍♂️ Movement relative to the player's facing direction
        Vector3 moveDirection =
            (transform.forward * input.MovementInput.z) + (transform.right * input.MovementInput.x);
        moveDirection.Normalize(); // Prevents diagonal speed boost

        // Ensure movement isn't zero before applying speed
        if (moveDirection.magnitude > 0)
        {
            Vector3 newPosition = rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }

        // Ground Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        // Jump
        if (input.JumpBuffered && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            input.JumpBuffered = false;
        }
    }
}
