using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using TMPro; // For UI Text Elements
using Unity.Services.Authentication;

public class MultiplayerUI : MonoBehaviour
{
    public TMP_Text JoinCodeText; // Shows Join Code for Clients
    public TMP_InputField joinCodeInput; // Input field for Clients to enter Join Code

    private UnityTransport transport;

    async void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // Initialize Unity Services (Must be done before using Relay)
        // ✅ Ensure Unity Services is initialized in standalone builds
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // ✅ Get device ID and sanitize it to only contain valid characters
        string rawDeviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;

        // ✅ Remove invalid characters (keep only a-z, A-Z, 0-9, -, and _)
        string sanitizedProfile = System.Text.RegularExpressions.Regex.Replace(
            rawDeviceId,
            "[^a-zA-Z0-9_-]",
            ""
        );

        // ✅ Ensure it does not exceed 30 characters
        if (sanitizedProfile.Length > 30)
        {
            sanitizedProfile = sanitizedProfile.Substring(0, 25);
            sanitizedProfile = sanitizedProfile + UnityEngine.Random.Range(1000, 9999);
            // sanitizedProfile = UnityEngine.Random.Range(1000, 9999).ToString();
        }

        Debug.Log($"Using sanitized profile: {sanitizedProfile}"); // ✅ Debugging log

        // ✅ Initialize Unity Services with the sanitized profile name
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync(
                new InitializationOptions().SetProfile(sanitizedProfile)
            );
        }

        // ✅ Ensure Authentication is signed in (Required for Relay)
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void StartHost()
    {
        try
        {
            Debug.Log($"Authenticated Player ID: {AuthenticationService.Instance.PlayerId}");

            // Allocate a Relay Server Slot for up to 4 players
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            // Generate a join code for clients to use
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Configure the Transport Layer to use Relay
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // Start hosting with Unity Netcode
            NetworkManager.Singleton.StartHost();

            // Display the Join Code for clients
            Debug.Log($"✅ Host started. Relay Join Code: {joinCode}");

            JoinCodeText.text = $"Relay Join Code: {joinCode} (Share this)";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Relay Error: {e.Message}");
        }
    }

    public async void StartClient()
    {
        try
        {
            Debug.Log($"Authenticated Player ID: {AuthenticationService.Instance.PlayerId}");

            string joinCode = joinCodeInput.text; // Get Join Code from UI
            Debug.Log($"🔍 Attempting to join Relay session with code: {joinCode}");

            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("❌ Please enter a valid join code!");
                return;
            }

            // Join Relay Server using the Join Code
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(
                joinCode
            );

            // Configure Transport Layer for Relay
            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            // Start as Client
            NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Relay Client Error: {e.Message}");
        }
    }
}
