using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;
using UnityEngine.UI;

public class NetworkLogger : MonoBehaviour
{
    public TMP_Text logText; // Assign in Inspector
    private string logContent = "***Press Tab to Toggle Networking UI (After Spawning)***";
    private int maxLines = 75; // Limit logs to avoid performance issues
    public ScrollRect scrollRect; // Reference the ScrollRect
    private static NetworkLogger instance;

    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this game object
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Otherwise, mark this as the active instance
        instance = this;

        // Prevent this object from being destroyed when loading new scenes
        DontDestroyOnLoad(gameObject);

        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Filter unsupported characters
        logString = RemoveUnsupportedCharacters(logString);

        // Format log
        logContent += $"\n[{type}] {logString}";

        // Keep only the last maxLines messages
        string[] lines = logContent.Split('\n');
        if (lines.Length > maxLines)
        {
            logContent = string.Join("\n", lines.Skip(lines.Length - maxLines));
        }

        // Update UI
        if (logText != null)
        {
            logText.text = logContent;
            Canvas.ForceUpdateCanvases(); // Force UI update

            ScrollToBottom();
        }
    }

    void ScrollToBottom()
    {
        // Ensure the layout updates before setting position
        Canvas.ForceUpdateCanvases();

        // Set content position to bottom
        scrollRect.verticalNormalizedPosition = 0f;

        // If that doesn’t work, manually force the content to move down
        scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, 0);
    }

    string RemoveUnsupportedCharacters(string input)
    {
        StringBuilder filteredText = new StringBuilder();

        foreach (char c in input)
        {
            if (logText.font.HasCharacter(c)) // Check if TMP font supports this character
            {
                filteredText.Append(c);
            }
            else
            {
                filteredText.Append('?'); // Replace unsupported characters
            }
        }

        return filteredText.ToString();
    }
}
