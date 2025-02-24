using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterProfileButton : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Optional: Text component displaying the character name.")]
    public TMP_Text characterNameText;

    [Tooltip("Optional: Image component displaying the character portrait.")]
    public Image characterPortrait;

    [Tooltip("Image component used as the highlight border.")]
    public Image highlightBorder;

    [Tooltip("3d model used in rotatingviewer")]
    public GameObject characterModel;

    [Header("Highlight Settings")]
    public Color selectedColor = Color.yellow; // or a metallic color
    public Color defaultColor = Color.clear; // transparent when not selected

    private Button button;

    private void Awake()
    {
        // Get the Button component (ensure it's on the same GameObject)
        button = GetComponent<Button>();
        if (button == null)
            Debug.LogError("No Button component found on " + gameObject.name);

        // Ensure the border is set to default (unhighlighted)
        if (highlightBorder != null)
            highlightBorder.gameObject.SetActive(false);

        if (characterModel != null)
        {
            characterModel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles the highlight border.
    /// </summary>
    public void SetHighlight(bool isSelected)
    {
        if (highlightBorder != null)
        {
            highlightBorder.gameObject.SetActive(isSelected);
            // Optionally, change its color
            highlightBorder.color = isSelected ? selectedColor : defaultColor;
        }
        if (characterModel != null)
        {
            characterModel.gameObject.SetActive(isSelected);
        }
    }

    /// <summary>
    /// Returns the Button component for adding click listeners.
    /// </summary>
    public Button GetButton()
    {
        return button;
    }
}
