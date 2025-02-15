using UnityEngine;
using Unity.Netcode;

public class PlayerSetup : NetworkBehaviour
{
    private CameraManager cameraManager;

    void Start()
    {
        InitializePlayer();
        if (!IsOwner)
            return; // 🛑 Only initialize for the local player

        Debug.Log("🔧 Player Setup: Running initialization for local player.");

        // Get random spawn position
        Transform spawnPoint = SpawnManager.Instance?.GetRandomSpawnPoint();
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            Debug.Log($"🚀 Player spawned at {spawnPoint.position}");
        }
        else
        {
            Debug.LogError("❌ No valid spawn point found!");
        }

        // Assign the camera
        cameraManager = FindFirstObjectByType<CameraManager>();
        if (cameraManager != null)
        {
            cameraManager.AssignCameraToPlayer(gameObject);
        }
        else
        {
            Debug.LogError("❌ CameraManager not found in the scene!");
        }

        // Additional future setup (name, skins, UI, etc.)
    }

    void InitializePlayer()
    {
        Debug.Log($"🎭 Initializing player: {gameObject.name}");

        PlayerAnimatorController animatorController = GetComponent<PlayerAnimatorController>();
        if (animatorController == null)
        {
            Debug.LogError("❌ PlayerAnimatorController is MISSING! Adding it manually.");
            animatorController = gameObject.AddComponent<PlayerAnimatorController>();
        }
    }
}
