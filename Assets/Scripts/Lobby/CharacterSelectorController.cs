using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSelectorController : MonoBehaviour
{
    // Option 1: Drag and drop the CharacterProfileButton components in the Inspector.
    //public List<CharacterProfileButton> profileButtons;

    // Option 2: Alternatively, auto-find them if they are children:
    private List<CharacterProfileButton> profileButtons;

    private CharacterProfileButton selectedProfile;

    private void Start()
    {
        // If you prefer to auto-find, uncomment the following:
        profileButtons = new List<CharacterProfileButton>(
            GetComponentsInChildren<CharacterProfileButton>()
        );

        // Add a click listener for each profile.
        foreach (CharacterProfileButton profile in profileButtons)
        {
            // Use a local copy for the lambda closure.
            CharacterProfileButton currentProfile = profile;
            currentProfile.GetButton().onClick.AddListener(() => OnProfileClicked(currentProfile));
        }

        // Highlight the first profile by default.
        if (profileButtons.Count > 0)
            OnProfileClicked(profileButtons[0]);
    }

    /// <summary>
    /// Called when a character profile is clicked.
    /// </summary>
    /// <param name="clickedProfile">The profile that was clicked.</param>
    private void OnProfileClicked(CharacterProfileButton clickedProfile)
    {
        // Unhighlight the currently selected profile.
        if (selectedProfile != null)
            selectedProfile.SetHighlight(false);

        // Mark the clicked profile as the current selection.
        selectedProfile = clickedProfile;
        selectedProfile.SetHighlight(true);

        // Determine the new character ID based on the button's position in the list.
        int newCharacterId = profileButtons.IndexOf(clickedProfile);

        // Update the local LobbyPlayer's network variable.
        var localLobbyPlayer =
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<LobbyPlayer>();
        if (localLobbyPlayer != null)
        {
            localLobbyPlayer.selectedCharacterId.Value = newCharacterId;
        }
        else
        {
            Debug.LogWarning("Local LobbyPlayer not found!");
        }

        // Optionally update the UI entry immediately.
        LobbyController lobbyController = FindFirstObjectByType<LobbyController>();
        if (lobbyController != null)
        {
            lobbyController.OnCharacterSelected(newCharacterId);
        }
        else
        {
            Debug.LogWarning("LobbyController not found in scene!");
        }

        Debug.Log(
            "Selected character: "
                + (
                    clickedProfile.characterNameText != null
                        ? clickedProfile.characterNameText.text
                        : "Unnamed"
                )
                + " with ID "
                + newCharacterId
        );
    }
}
