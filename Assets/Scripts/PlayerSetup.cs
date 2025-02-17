using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class PlayerSetup : NetworkBehaviour
{
    private CameraManager cameraManager;
    private Rigidbody rb;

    private NetworkTransform nt;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nt = GetComponent<NetworkTransform>();
        InitializePlayer();
        if (!IsOwner)
            return; // 🛑 Only initialize for the local player

        Debug.Log("🔧 Player Setup: Running initialization for local player.");

        // Get random spawn position
        Transform spawnPoint = SpawnManager.Instance?.GetRandomSpawnPoint();
        if (spawnPoint != null)
        {
            Debug.Log($"🚀 Spawning player at {spawnPoint.position}");

            // 🛠️ **Step 1: Temporarily Disable Rigidbody Physics**
            rb.isKinematic = true; // Prevents physics interference

            // 🛰️ **Step 2: Move the Player to the Correct Position**
            rb.MovePosition(spawnPoint.position);
            nt.Teleport(spawnPoint.position, spawnPoint.rotation, transform.localScale);

            Debug.Log($"📌 rb.MovePosition() called! Moving to {spawnPoint.position}");
            transform.rotation = spawnPoint.rotation;

            // 🎮 **Step 3: Reactivate Physics After Positioning**
            Invoke(nameof(EnablePhysics), 0.1f); // Small delay to prevent issues
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

    void EnablePhysics()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Re-enable physics
            Debug.Log($"🟢 Rigidbody is now active! Current Position: {transform.position}");
        }
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
