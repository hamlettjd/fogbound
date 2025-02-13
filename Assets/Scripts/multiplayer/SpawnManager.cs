using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance; // Singleton for easy access

    public List<Transform> spawnPoints = new List<Transform>(); // List of spawn points

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError(
                "🚨 SpawnManager: No spawn points assigned! Players may not spawn correctly."
            );
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("❌ SpawnManager: No spawn points available!");
            return null;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }
}
