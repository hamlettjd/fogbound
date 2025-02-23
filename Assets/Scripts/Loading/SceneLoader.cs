using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : NetworkBehaviour
{
    public static SceneLoader Instance;
    public GameObject loadingCanvas; // Reference to the loading UI

    [HideInInspector]
    public bool isLoadingScene = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        // Ensure loading canvas isn't visible by default
        loadingCanvas.SetActive(false);
    }

    // // Listen for scene events upon network start (server only)
    // public override void OnNetworkStart()
    // {
    //     if (IsServer)
    //     {
    //         NetworkManager.SceneManager.SceneEvent += OnSceneEvent;
    //     }
    // }

    // // Cleanup subscriptions when the network object is destroyed
    // public override void OnNetworkDestruction()
    // {
    //     if (NetworkManager.Singleton != null)
    //     {
    //         NetworkManager.SceneManager.SceneEvent -= OnSceneEvent;
    //     }
    // }

    [ServerRpc(RequireOwnership = false)]
    public void StartSceneLoadServerRpc(string sceneName)
    {
        if (IsServer)
        {
            ShowLoadingCanvas();
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    private void ShowLoadingCanvas()
    {
        loadingCanvas.SetActive(true); // Display the loading UI
    }

    private void HideLoadingCanvas()
    {
        loadingCanvas.SetActive(false); // Hide the loading UI
    }

    // Optionally, simulate loading progress if needed
    public void SimulateLoadingProgress()
    {
        if (isLoadingScene)
        {
            float progress = Mathf.PingPong(Time.time / 5f, 1f); // Simulated progress value
            LoadingUI.Instance.UpdateProgress(progress);
        }
    }
}
