using Unity.Netcode;
//using Unity.Netcode.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    private GameObject loadingCanvas; // Assign your loading canvas in the Inspector
    private Slider progressBar; // Optional: assign a Slider for progress updates

    private void Awake()
    {
        // Find the LoadingCanvas game object in the scene
        loadingCanvas = GameObject.Find("LoadingCanvas");
        if (loadingCanvas == null)
        {
            Debug.LogError("LoadingCanvas not found in the scene!");
            return;
        }

        // Find the ProgressBar child under LoadingCanvas
        Transform progressBarTransform = loadingCanvas.transform.Find("ProgressBar");
        if (progressBarTransform == null)
        {
            Debug.LogError("ProgressBar not found as a child of LoadingCanvas!");
            return;
        }

        // Get the Slider component from the ProgressBar game object
        progressBar = progressBarTransform.GetComponent<Slider>();
        if (progressBar == null)
        {
            Debug.LogError("No Slider component found on ProgressBar!");
        }
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;
        }
        loadingCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;
        }
    }

    private void HandleSceneEvent(SceneEvent sceneEvent)
    {
        // Depending on the event type, update the loading UI.
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.Load:
                // When a scene load starts, show the loading canvas.
                loadingCanvas.SetActive(true);
                if (progressBar != null)
                    progressBar.value = 0; // Reset progress (if using a progress bar)
                break;

            case SceneEventType.LoadComplete:
                // When the scene has finished loading, hide the canvas.
                loadingCanvas.SetActive(false);
                break;

            // Optionally, if you want to simulate progress:
            // case SceneEventType.Progress:
            //     // You could update the progress bar here if you had a value.
            //     break;
        }
    }
}
