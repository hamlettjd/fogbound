using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    // Start with just the name; add more fields as needed
    public string playerName;
    public int playerCharacterId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
