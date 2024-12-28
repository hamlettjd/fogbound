using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;

    private PlayerMovement movement;

    public float previousSpeed = 0;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        if (animator == null)
        {
            Debug.LogWarning("Animator is not assigned.");
        }
    }

    void Update()
    {
        if (animator == null)
            return;

        // Update speed
        if (previousSpeed != movement.currentSpeed)
        {
            Debug.Log($"speed has changed to: {movement.currentSpeed}");
        }
        animator.SetFloat("Speed", movement.currentSpeed / movement.maxRunSpeed);

        // Update grounded and jumping states
        animator.SetBool("IsGrounded", movement.isGrounded);
        previousSpeed = movement.currentSpeed;
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
