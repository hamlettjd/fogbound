using Unity.Netcode;
using UnityEngine;
using TMPro; // ✅ Required for TextMesh Pro

public class MultiplayerUI : MonoBehaviour
{
    public TMP_Text hostButtonText;
    public TMP_Text clientButtonText;
    public TMP_Text statusText; // Optional: Display connection status

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        UpdateStatus("Hosting Game...");
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        UpdateStatus("Joining Game...");
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
