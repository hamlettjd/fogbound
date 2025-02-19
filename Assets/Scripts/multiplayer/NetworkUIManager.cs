using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class NetworkUIManager : MonoBehaviour
{
    public InputField ipAddressInput; // Assign this in Unity for client to input the host's IP
    public Text hostIpDisplay; // Assign this in Unity to display the host's IP
    private UnityTransport transport;

    void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // Display the host's IP when hosting
        if (hostIpDisplay != null)
        {
            hostIpDisplay.text = $"Your IP: {GetLocalIPAddress()}";
        }
    }

    public void StartHost()
    {
        transport.SetConnectionData(GetLocalIPAddress(), 7777); // Use a consistent port (7777)
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        string ip = ipAddressInput.text; // Read IP from UI input field
        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogError("❌ IP Address is empty! Please enter the host's IP.");
            return;
        }

        transport.SetConnectionData(ip, 7777);
        NetworkManager.Singleton.StartClient();
    }

    private string GetLocalIPAddress()
    {
        // Gets the machine's local network IP (not public)
        string localIP = "127.0.0.1"; // Default fallback
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
}
