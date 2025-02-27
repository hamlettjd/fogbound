using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerCharacter : NetworkBehaviour
{
    public TMP_Text playerNameText; // Assign in Inspector
    private string playerName; // No longer a NetworkVariable

    public void SetPlayerName(string name)
    {
        playerName = name;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }
    }
}
