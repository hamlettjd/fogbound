using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerCharacter : NetworkBehaviour
{
    public TMP_Text playerNameText; // Assign in Inspector
    private string playerName;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // Find the main camera
    }

    private void Update()
    {
        if (playerNameText != null)
        {
            // Make the name tag face the camera
            playerNameText.transform.rotation = Quaternion.LookRotation(
                playerNameText.transform.position - mainCamera.transform.position
            );
        }
    }

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
