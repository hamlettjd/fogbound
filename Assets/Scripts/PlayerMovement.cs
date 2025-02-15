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

    private NetworkVariable<Vector3> networkedPosition = new NetworkVariable<Vector3>(
        Vector3.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        currentSpeed = walkSpeed;
    }

    void FixedUpdate()
    {
        if (!IsOwner)
        {
            // 🛑 Non-owners should only read position updates, NOT move the object
            transform.position = networkedPosition.Value;
            return;
        }

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

        // Ground Check
        RaycastHit hit;
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, groundLayer);

        // Jump
        if (input.JumpBuffered && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            input.JumpBuffered = false;
        }

        // 🛰️ Update the position over the network
        networkedPosition.Value = transform.position;
    }
}
