using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro; // ✅ Required for TextMesh Pro
using System.Net;
using System.Net.Sockets;

public class MultiplayerUI : MonoBehaviour
{
    public TMP_InputField ipAddressInput; // 🆕 Input field for Client to enter IP
    public TMP_Text hostIpText; // 🆕 Display the Host's IP address
    public TMP_Text statusText; // Status updates for UI

    private UnityTransport transport;

    void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // 🏠 Display the host's IP when starting the game
        if (hostIpText != null)
        {
            hostIpText.text = $"Your IP: {GetLocalIPAddress()}";
        }
    }

    public void StartHost()
    {
        string localIP = GetLocalIPAddress();
        transport.SetConnectionData(localIP, 7777); // Use port 7777
        NetworkManager.Singleton.StartHost();
        UpdateStatus("Hosting Game... Share your IP: " + localIP);
    }

    public void StartClient()
    {
        string ip = ipAddressInput.text; // Get entered IP from UI
        if (string.IsNullOrEmpty(ip))
        {
            UpdateStatus("❌ Enter Host IP!");
            return;
        }

        transport.SetConnectionData(ip, 7777);
        NetworkManager.Singleton.StartClient();
        UpdateStatus("Joining Game...");
    }

    private string GetLocalIPAddress()
    {
        // Retrieves the local network IP address (not public)
        string localIP = "127.0.0.1";
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
