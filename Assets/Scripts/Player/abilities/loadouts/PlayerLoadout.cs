using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections;
using System.Collections.Generic;

// Player Loadout: Determines which abilities a character has and handles input// Player Loadout: Determines which abilities a character has and handles input
public class PlayerLoadout : NetworkBehaviour
{
    public List<Power> AvailablePowers = new List<Power>();
    private PlayerInputActions inputActions;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log(
            $"[PlayerLoadout] OnNetworkSpawn() called. "
                + $"NetworkObject: {NetworkObject}, OwnerClientId: {NetworkObject?.OwnerClientId}, "
                + $"IsOwner: {IsOwner}, GameObject: {gameObject.name}"
        );
        if (!IsOwner)
        {
            Debug.Log(
                $"[PlayerLoadout] Not owner, disabling input on client {NetworkManager.LocalClientId}"
            );
            return;
        }
        Debug.Log($"[PlayerLoadout] Initializing input for owner {OwnerClientId}");
        if (IsOwner)
        {
            Debug.Log("Initializing Player Loadout...");

            // Ensure powers are assigned correctly on the local client
            // Find and store existing power components instead of adding them
            MeleeAttack meleeAttack = GetComponent<MeleeAttack>();
            MistbornSteelPushOmni steelPush = GetComponent<MistbornSteelPushOmni>();
            MistbornIronPullOmni IronPull = GetComponent<MistbornIronPullOmni>();
            MistbornBurnVisuals BurnVisuals = GetComponent<MistbornBurnVisuals>();

            if (meleeAttack != null)
            {
                AvailablePowers.Add(meleeAttack);
            }
            else
            {
                Debug.LogError("[PlayerLoadout] MeleeAttack component is missing on player!");
            }

            if (steelPush != null)
            {
                AvailablePowers.Add(steelPush);
            }
            else
            {
                Debug.LogError("[PlayerLoadout] MistbornSteelPush component is missing on player!");
            }
            if (IronPull != null)
            {
                AvailablePowers.Add(IronPull);
            }
            else
            {
                Debug.LogError("[PlayerLoadout] MistbornSteelPush component is missing on player!");
            }
            if (BurnVisuals != null)
            {
                AvailablePowers.Add(BurnVisuals);
            }
            else
            {
                Debug.LogError("[PlayerLoadout] MistbornSteelPush component is missing on player!");
            }
            Debug.Log($"Player Loadout initialized with {AvailablePowers.Count} powers.");

            inputActions = new PlayerInputActions();
            inputActions.Player.Enable();
        }
    }

    public void UsePower(int index)
    {
        Debug.Log(
            $"[PlayerLoadout] UsePower({index}) called by client {NetworkManager.LocalClientId}. "
                + $"OwnerClientId: {OwnerClientId}, IsOwner: {IsOwner}"
        );
        Debug.Log(
            $"[PlayerLoadout] Checking NetworkObject ownership. "
                + $"NetworkObject: {NetworkObject}, "
                + $"NetworkObject.OwnerClientId: {NetworkObject?.OwnerClientId}, "
                + $"IsOwner: {IsOwner}, "
                + $"GameObject: {gameObject.name}, Parent: {transform.parent?.name}"
        );
        if (!IsOwner) // Ensures only the player who owns this character can use a power
        {
            Debug.Log($"Ignoring UsePower({index}) because this player is not the owner.");
            return;
        }

        if (index < AvailablePowers.Count && AvailablePowers[index] != null)
        {
            Debug.Log(
                $"[PlayerLoadout] Using Power: {AvailablePowers[index].PowerName} (Index: {index}) "
                    + $"on Client {NetworkManager.LocalClientId}"
            );
            AvailablePowers[index].Activate();
        }
        else
        {
            Debug.LogError($"No power found at index {index}");
        }
    }

    private void OnEnable() => inputActions.Player.Enable();

    private void OnDisable() => inputActions.Player.Disable();
}
