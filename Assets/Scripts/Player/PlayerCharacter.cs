using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerCharacter : NetworkBehaviour
{
    public TMP_Text playerNameText; // Assign in Inspector
    private Camera mainCamera;

    public override void OnNetworkSpawn()
    {
        if (IsOwner) // Hide the local player's own nameplate
        {
            if (playerNameText != null)
            {
                playerNameText.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        FindMainCamera(); // Get the correct camera reference
    }

    private void Update()
    {
        if (mainCamera == null)
        {
            FindMainCamera(); // Ensure we get the camera if it's null
        }

        if (playerNameText != null && mainCamera != null)
        {
            FaceCameraYAxisOnly();
        }
    }

    private void FindMainCamera()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main;
        }
        else
        {
            Debug.LogWarning("Main camera not found for player name billboard.");
        }
    }

    private void FaceCameraYAxisOnly()
    {
        // Get direction from text to camera
        Vector3 direction = mainCamera.transform.position - playerNameText.transform.position;

        // Lock the Y-axis by zeroing out the Y component
        direction.y = 0;

        // Check if direction is valid (to prevent flickering)
        if (direction.sqrMagnitude > 0.001f)
        {
            playerNameText.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void SetPlayerName(string name)
    {
        if (playerNameText != null)
        {
            playerNameText.text = name;
        }
    }
}
