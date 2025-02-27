using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerCharacter : NetworkBehaviour
{
    public TMP_Text playerNameText; // Assign in Inspector
    private string playerName;
    private Camera mainCamera;

    public override void OnNetworkSpawn()
    {
        if (IsOwner) // If this character belongs to the local player
        {
            if (playerNameText != null)
            {
                playerNameText.gameObject.SetActive(false); // Hide name for self
            }
        }
    }

    private void Start()
    {
        mainCamera = Camera.main; // Find the main camera
    }

    private void Update()
    {
        if (playerNameText != null)
        {
            // Compute direction from text to camera
            Vector3 direction = playerNameText.transform.position - mainCamera.transform.position;
            // Zero out the y component to lock rotation on the horizontal plane
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
            {
                playerNameText.transform.rotation = Quaternion.LookRotation(direction);
            }
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
