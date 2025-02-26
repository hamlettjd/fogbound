using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

// [GenerateSerializationForType(typeof(string))]
public class LobbyPlayer : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> playerName =
        new NetworkVariable<FixedString32Bytes>();

    // For example, an integer that represents the selected character (e.g., 0 = Knight, 1 = Archer, etc.)
    public NetworkVariable<int> selectedCharacterId = new NetworkVariable<int>(1);

    // You can add additional networked data here as needed.

    public override void OnNetworkSpawn()
    {
        // Only the owning client needs to send their name when they join.
        if (IsOwner)
        {
            playerName.Value = GameData.Instance.playerName;
            //selectedCharacterId = GameData.Instance.playerCharacterId;
            Debug.Log($"PlayerName in lobbyPlayer is: {playerName.Value}");
            selectedCharacterId.Value = GameData.Instance.playerCharacterId; // Make sure this value is valid (0 or 1)
            Debug.Log(
                $"[LobbyPlayer] Setting selectedCharacterId for client {OwnerClientId} to {selectedCharacterId.Value}"
            );
        }
        selectedCharacterId.OnValueChanged += OnSelectedCharacterIdChanged;
        selectedCharacterId.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log(
                $"[LobbyPlayer] selectedCharacterId changed from {oldValue} to {newValue} for client {OwnerClientId}"
            );
        };
    }

    private void OnSelectedCharacterIdChanged(int oldId, int newId)
    {
        // Update the UI for this LobbyPlayer instance.
        // This could involve finding the corresponding player entry UI and updating it.
        LobbyController lobbyController = FindFirstObjectByType<LobbyController>();
        if (lobbyController != null)
        {
            lobbyController.UpdatePlayerEntryUI(OwnerClientId, newId);
        }
    }
}
