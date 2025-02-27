using System.Collections.Generic;
using UnityEngine;

public class GameCharacterManager : MonoBehaviour
{
    public static GameCharacterManager Instance;

    public List<GameObject> characterPrefabs; // Assign in Unity Editor

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetCharacterPrefab(int characterId)
    {
        if (characterId >= 0 && characterId < characterPrefabs.Count)
        {
            return characterPrefabs[characterId];
        }
        Debug.LogError($"Invalid character ID {characterId}, defaulting to 0.");
        return characterPrefabs[0]; // Default character
    }
}
