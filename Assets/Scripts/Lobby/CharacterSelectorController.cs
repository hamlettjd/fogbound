using System.Collections.Generic;
using UnityEngine;

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

        // TODO: Update the current character selection (e.g., notify the network manager, update player info, etc.)
        Debug.Log(
            "Selected character: "
                + (
                    selectedProfile.characterNameText != null
                        ? selectedProfile.characterNameText.text
                        : "Unnamed"
                )
        );
    }
}
