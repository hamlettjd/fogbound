using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
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
        // Check ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

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

        // Jump
        if (input.JumpInput && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
