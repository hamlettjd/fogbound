using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance;

    [SerializeField]
    private Slider progressBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdateProgress(float progress)
    {
        progressBar.value = progress;
    }
}
