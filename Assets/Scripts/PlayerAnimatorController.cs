using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;

    private PlayerMovement movement;
    private PlayerInput input;
    private Rigidbody rb; // Reference to Rigidbody for velocity calculations

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>(); // Get Rigidbody from Player GameObject
        if (animator == null)
        {
            Debug.LogWarning("Animator is not assigned.");
        }
    }

    void Update()
    {
        if (animator == null)
            return;

        // Ensure speed transitions smoothly but properly goes to zero when not moving
        if (input.MovementInput.magnitude > 0)
        {
            animator.SetFloat("Speed", movement.currentSpeed / movement.maxRunSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0); // Force idle animation when not moving
        }
        // **Trigger Jump when the player leaves the ground**
        if (!movement.isGrounded && !animator.GetBool("IsJumping"))
        {
            TriggerJump(); // Call the function to start jump animation
        }
    }

    void FixedUpdate()
    {
        if (animator == null)
            return;

        // Grounded state should be updated in FixedUpdate since it's physics-dependent
        animator.SetBool("IsGrounded", movement.isGrounded);

        // Update FallSpeed dynamically based on vertical velocity
        if (!movement.isGrounded)
        {
            float fallSpeed = 1 + Mathf.Abs(rb.linearVelocity.y);
            animator.SetFloat("FallSpeed", fallSpeed);
        }
        else if (animator.GetBool("IsJumping")) // If grounded and was jumping, reset the jump layer
        {
            Debug.Log("starting resetJumpLayer");
            StartCoroutine(ResetJumpLayer());
        }
    }

    public void TriggerJump()
    {
        if (animator != null)
        {
            animator.SetLayerWeight(1, 1f);
            animator.SetBool("IsJumping", true);
            animator.Play("Jump Start", 1);
        }
    }

    public IEnumerator ResetJumpLayer()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        while (!stateInfo.IsName("Jump Land") || stateInfo.normalizedTime < 1f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(1);
            yield return null;
        }

        animator.SetLayerWeight(1, 0f);
        animator.SetBool("IsJumping", false);
    }
}
