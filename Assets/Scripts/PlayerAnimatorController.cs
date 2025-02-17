using UnityEngine;
using System.Collections;
using Unity.Netcode;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimatorController : NetworkBehaviour
{
    public Animator animator;

    private PlayerMovement movement;
    private PlayerInput input;
    private Rigidbody rb;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            Debug.LogWarning("Animator is not assigned.");
        }
    }

    void Update()
    {
        if (animator == null)
            Debug.LogWarning("⚠️ Animator is not assigned!");
        return;
        // **✅ Local Player: Instantly update animations**

        // 🎭 Log animation values to check if they update correctly
        Debug.Log(
            $"🎭 [Local Player Anim Debug] Speed: {animator.GetFloat("Speed")}, IsJumping: {animator.GetBool("IsJumping")}, IsGrounded: {animator.GetBool("IsGrounded")}"
        );
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
            TriggerJump();
        }
    }

    void FixedUpdate()
    {
        if (animator == null)
            return;

        animator.SetBool("IsGrounded", movement.isGrounded);

        if (!movement.isGrounded)
        {
            float fallSpeed = 1 + Mathf.Abs(rb.linearVelocity.y);
            animator.SetFloat("FallSpeed", fallSpeed);
        }
        else if (animator.GetBool("IsJumping"))
        {
            StartCoroutine(ResetJumpLayer());
        }
    }

    // 🔹 **Called when speed updates over the network (for remote players)**
    public void UpdateSpeed(float speed)
    {
        if (!IsOwner) // 🔹 Only remote players get updates
        {
            animator.SetFloat("Speed", speed);
        }
    }

    // 🔹 **Called when jump state updates over the network (for remote players)**
    public void UpdateJumpState(bool isJumping)
    {
        if (!IsOwner)
        {
            if (isJumping)
                TriggerJump();
            else
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
