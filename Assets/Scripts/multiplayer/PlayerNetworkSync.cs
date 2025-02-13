using Unity.Netcode;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAnimatorController))]
public class PlayerNetworkSync : NetworkBehaviour
{
    private PlayerMovement movement;
    private PlayerAnimatorController animatorController;

    // 🔄 Network Variables for Syncing Player State
    private NetworkVariable<float> netSpeed = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private NetworkVariable<bool> netIsJumping = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    // ✅ EVENTS for animation updates
    public event Action<float> OnSpeedChanged;
    public event Action<bool> OnJumpStateChanged;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        animatorController = GetComponent<PlayerAnimatorController>();

        if (!IsOwner)
            return;

        movement.OnSpeedChanged += SyncSpeed;
        movement.OnJumpStateChanged += SyncJump;
    }

    private void SyncSpeed(float speed)
    {
        if (Mathf.Abs(netSpeed.Value - speed) > 0.01f)
        {
            netSpeed.Value = speed;
        }
    }

    private void SyncJump(bool isJumping)
    {
        if (netIsJumping.Value != isJumping)
        {
            netIsJumping.Value = isJumping;
        }
    }

    private void OnEnable()
    {
        netSpeed.OnValueChanged += (oldValue, newValue) =>
        {
            OnSpeedChanged?.Invoke(newValue); // Trigger event
            animatorController.UpdateSpeed(newValue);
        };

        netIsJumping.OnValueChanged += (oldValue, newValue) =>
        {
            OnJumpStateChanged?.Invoke(newValue); // Trigger event
            animatorController.UpdateJumpState(newValue);
        };
    }

    private void OnDisable()
    {
        netSpeed.OnValueChanged -= (oldValue, newValue) =>
        {
            OnSpeedChanged?.Invoke(newValue);
            animatorController.UpdateSpeed(newValue);
        };

        netIsJumping.OnValueChanged -= (oldValue, newValue) =>
        {
            OnJumpStateChanged?.Invoke(newValue);
            animatorController.UpdateJumpState(newValue);
        };
    }

    // 🔹 **ServerRPC Methods for Explicit Sync**
    [ServerRpc]
    public void SetSpeedServerRpc(float speed)
    {
        netSpeed.Value = speed;
    }

    [ServerRpc]
    public void SetJumpStateServerRpc(bool isJumping)
    {
        netIsJumping.Value = isJumping;
    }
}
