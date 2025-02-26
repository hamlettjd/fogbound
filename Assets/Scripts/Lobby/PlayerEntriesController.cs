using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerEntriesController : MonoBehaviour
{
    // Reference to the text element for displaying the player's name.
    public TMP_Text playerNameText;

    // Reference to the image element for displaying the character sprite.
    public Image characterImage;

    // Mapping from a selectedCharacterId to a Sprite.
    // Configure this list in the Inspector so that index 0 corresponds to sprite for character 0, index 1 for character 1, etc.
    public List<Sprite> characterSprites;

    /// <summary>
    /// Sets the player's name on the UI entry.
    /// </summary>
    public void SetPlayerName(string playerName)
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }
        else
        {
            Debug.LogWarning("PlayerNameText is not assigned on " + gameObject.name);
        }
    }

    /// <summary>
    /// Updates the character image based on the selectedCharacterId.
    /// </summary>
    public void SetCharacterId(int selectedCharacterId)
    {
        Debug.Log("inside SetCharacterId function");
        Sprite spriteToUse = null;
        if (
            characterSprites != null
            && selectedCharacterId >= 0
            && selectedCharacterId < characterSprites.Count
        )
        {
            spriteToUse = characterSprites[selectedCharacterId];
        }
        else
        {
            Debug.LogWarning(
                "Invalid selectedCharacterId (" + selectedCharacterId + ") on " + gameObject.name
            );
        }

        if (characterImage != null)
        {
            characterImage.sprite = spriteToUse;
        }
        else
        {
            Debug.LogWarning("CharacterImage is not assigned on " + gameObject.name);
        }
    }
}
