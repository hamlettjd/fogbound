using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

// Include networking namespaces as needed (e.g., using Unity.Netcode)

public class LobbyController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Content container of the PlayerListScrollView.")]
    public Transform playerListContent;

    [Tooltip("The prefab for a single player entry.")]
    public GameObject playerEntryPrefab;

    [Tooltip("The Start Game button (only visible to the host).")]
    public Button startGameButton;

    // Dictionary to keep track of player entries (key could be player ID or name)
    private Dictionary<ulong, GameObject> playerEntries = new Dictionary<ulong, GameObject>();

    private void Start()
    {
        // Determine if the local client is the host.
        // For example, if using Unity Netcode:
        // bool isHost = NetworkManager.Singleton.IsHost;
        // For this example, let's assume a dummy value:
        bool isHost = true; // Replace with your actual check

        // Only the host should see the Start Game button.
        startGameButton.gameObject.SetActive(isHost);

        // (Optional) Add a listener for the Start Game button.
        startGameButton.onClick.AddListener(OnStartGameClicked);

        // Subscribe to the event when a client connects
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        // In case the host is already connected, add that player too
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            AddPlayer(client.ClientId);
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
    }

    private void HandleClientConnected(ulong clientId)
    {
        AddPlayer(clientId);
    }

    /// <summary>
    /// Adds a player entry to the player list.
    /// </summary>
    /// <param name="playerName">The player's display name.</param>
    /// <param name="characterSprite">The sprite of the character the player has selected (can be null).</param>
    public void AddPlayer(ulong clientId)
    {
        var networkClient = NetworkManager.Singleton.ConnectedClients[clientId];
        if (networkClient.PlayerObject != null)
        {
            var lobbyPlayer = networkClient.PlayerObject.GetComponent<LobbyPlayer>();
            if (lobbyPlayer != null)
            {
                // Create a UI element (playerListItem) for the connected client.
                GameObject listItem = Instantiate(playerEntryPrefab, playerListContent);
                // Assume playerListItem has a script to set its UI, e.g., PlayerListItemController.
                var itemController = listItem.GetComponent<PlayerEntriesController>();
                if (itemController != null)
                {
                    Debug.Log($"B - player name is: {lobbyPlayer.playerName.Value}");
                    Debug.Log(
                        $"C - player name ToString() is: {lobbyPlayer.playerName.Value.ToString()}"
                    );
                    itemController.SetPlayerName(lobbyPlayer.playerName.Value.ToString());
                    itemController.SetCharacterId(lobbyPlayer.selectedCharacterId.Value);
                }
                playerEntries.Add(clientId, listItem);
            }
        }
    }

    // Called from the UI when a client (the local player) selects a different character.
    // This function updates the local player's network variable.
    public void OnCharacterSelected(int newCharacterId)
    {
        var localLobbyPlayer =
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<LobbyPlayer>();
        if (localLobbyPlayer != null)
        {
            localLobbyPlayer.selectedCharacterId.Value = newCharacterId;
            // Optionally, update the local UI immediately.
            if (
                playerEntries.TryGetValue(
                    NetworkManager.Singleton.LocalClientId,
                    out GameObject listItem
                )
            )
            {
                var itemController = listItem.GetComponent<PlayerEntriesController>();
                if (itemController != null)
                    itemController.SetCharacterId(newCharacterId);
            }
        }
    }

    public void UpdatePlayerEntryUI(ulong clientId, int newCharacterId)
    {
        if (playerEntries.TryGetValue(clientId, out GameObject listItem))
        {
            var itemController = listItem.GetComponent<PlayerEntriesController>();
            if (itemController != null)
            {
                itemController.SetCharacterId(newCharacterId);
            }
        }
    }

    /// <summary>
    /// Removes a player from the player list.
    /// </summary>
    public void RemovePlayer(ulong clientId)
    {
        if (playerEntries.TryGetValue(clientId, out GameObject entry))
        {
            Destroy(entry);
            playerEntries.Remove(clientId);
        }
    }

    /// <summary>
    /// Called when the host clicks the Start Game button.
    /// </summary>
    private void OnStartGameClicked()
    {
        // Implement your logic to transition from lobby to game scene.
        // For example, using Unity Netcode:
        Debug.Log("Start Game button clicked!");
        NetworkManager.Singleton.SceneManager.LoadScene("knightScene", LoadSceneMode.Single);
    }
}
