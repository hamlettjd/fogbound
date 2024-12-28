using UnityEngine;
using System.Collections;

public class PlayerMovementOld : MonoBehaviour
{
    public float walkSpeed = 4f; // Default walking speed
    public float maxRunSpeed = 10f; // Maximum running speed
    public float acceleration = 10f; // Acceleration when sprinting
    public float jumpForce = 5f; // Jump force
    public float currentSpeed; // Current movement speed
    private Rigidbody rb;
    private bool isGrounded;
    public LayerMask groundLayer;
    public Animator animator; // Reference to the Animator component

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = walkSpeed; // Initialize current speed to walking speed
        if (animator == null)
        {
            Debug.LogWarning("Animator is not assigned. Assign the Animator in the Inspector.");
        }
    }

    void Update()
    {
        Debug.Log($"Current State: {animator.GetCurrentAnimatorStateInfo(1).fullPathHash}");
        Debug.Log($"Layer Weight: {animator.GetLayerWeight(1)}");
        // Movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        // Adjust speed for running or walking
        if (Input.GetKey(KeyCode.LeftShift) && movement.magnitude > 0) // Sprinting
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                maxRunSpeed,
                acceleration * Time.deltaTime
            );
        }
        else // Walking
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                walkSpeed,
                acceleration * Time.deltaTime
            );
        }

        // Apply movement
        transform.Translate(movement * currentSpeed * Time.deltaTime, Space.Self);

        // Update Animator with movement speed
        if (animator != null)
        {
            animator.SetFloat("Speed", movement.magnitude * (currentSpeed / maxRunSpeed));
        }

        // Ground Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        // Update Animator parameters
        if (animator != null)
        {
            animator.SetBool("IsGrounded", isGrounded);

            // Adjust animation speed for Jump Mid based on fall speed
            if (!isGrounded)
            {
                float fallSpeed = Mathf.Abs(rb.linearVelocity.y);
                animator.SetFloat("FallSpeed", fallSpeed);
            }
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !animator.GetBool("IsJumping"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (animator != null)
            {
                animator.SetLayerWeight(1, 1f); // Activate Jump Layer
                animator.SetBool("IsJumping", true);

                // Force the Jump Start state
                if (!animator.GetCurrentAnimatorStateInfo(1).IsName("Jump Start"))
                {
                    animator.Play("Jump Start", 1); // Jump Layer, Jump Start state
                }
            }
        }
    }

    private IEnumerator ResetJumpLayerAfterAnimation()
    {
        // Wait for Jump Land animation to finish
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1); // Jump Layer
        while (!stateInfo.IsName("Jump Land") || stateInfo.normalizedTime < 1f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(1);
            yield return null;
            Debug.Log($"stateInfo.normalizedTime : {stateInfo.normalizedTime}");
        }

        // Reset Jump Layer and parameters
        animator.SetLayerWeight(1, 0f);
        animator.SetBool("IsJumping", false);
    }
}
