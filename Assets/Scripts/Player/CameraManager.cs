using UnityEngine;
using Unity.Netcode;

public class CameraManager : MonoBehaviour
{
    private Camera lobbyCamera; // Reference to the scene's initial camera
    private CameraController cameraController;

    void Start()
    {
        // Find and store the reference to the Lobby Camera (if it exists)
        lobbyCamera = GameObject.Find("LobbyCamera")?.GetComponent<Camera>();
        cameraController = FindFirstObjectByType<CameraController>();
    }

    public void AssignCameraToPlayer(GameObject player)
    {
        if (cameraController == null)
        {
            Debug.LogError("❌ CameraController not found in scene!");
            return;
        }

        if (IsLocalPlayer(player))
        {
            Debug.Log("📷 Assigning camera to local player.");
            cameraController.AssignPlayer(player.transform);

            if (lobbyCamera != null)
            {
                Debug.Log("🎥 Disabling Lobby Camera.");
                lobbyCamera.gameObject.SetActive(false);
            }
        }
    }

    private bool IsLocalPlayer(GameObject player)
    {
        // Ensure it's a NetworkObject and check if it's owned by the local player
        NetworkObject netObj = player.GetComponent<NetworkObject>();
        return netObj != null && netObj.IsOwner;
    }
}
