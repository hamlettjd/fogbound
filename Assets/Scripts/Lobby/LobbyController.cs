using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private Dictionary<string, GameObject> playerEntries = new Dictionary<string, GameObject>();

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

        // (Optional) Populate the lobby with current players.
        // This would typically come from your networking layer.
        // For demonstration, let's add a couple of dummy players.
        AddPlayer("Player1", null);
        AddPlayer("Player2", null);
    }

    /// <summary>
    /// Adds a player entry to the player list.
    /// </summary>
    /// <param name="playerName">The player's display name.</param>
    /// <param name="characterSprite">The sprite of the character the player has selected (can be null).</param>
    public void AddPlayer(string playerName, Sprite characterSprite)
    {
        if (playerEntries.ContainsKey(playerName))
            return;

        // Instantiate the prefab as a child of the content container.
        GameObject entry = Instantiate(playerEntryPrefab, playerListContent);

        // Set the player's name.
        TMP_Text nameText = entry.transform.Find("PlayerNameText").GetComponent<TMP_Text>();
        if (nameText != null)
        {
            nameText.text = playerName;
        }
        else
        {
            Debug.LogWarning("PlayerNameText not found on prefab.");
        }

        // Set the character image.
        Image charImage = entry.transform.Find("CharacterImage").GetComponent<Image>();
        if (charImage != null)
        {
            charImage.sprite = characterSprite; // This can be null if no sprite is provided.
        }
        else
        {
            Debug.LogWarning("CharacterImage not found on prefab.");
        }

        // Save reference for potential future removal/updates.
        playerEntries.Add(playerName, entry);
    }

    /// <summary>
    /// Removes a player from the player list.
    /// </summary>
    public void RemovePlayer(string playerName)
    {
        if (playerEntries.TryGetValue(playerName, out GameObject entry))
        {
            Destroy(entry);
            playerEntries.Remove(playerName);
        }
    }

    /// <summary>
    /// Called when the host clicks the Start Game button.
    /// </summary>
    private void OnStartGameClicked()
    {
        // Implement your logic to transition from lobby to game scene.
        // For example, using Unity Netcode:
        // NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);

        Debug.Log("Start Game button clicked!");
    }
}
