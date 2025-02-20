using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector3 MovementInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool JumpBuffered { get; set; }
    private GameObject multiplayerUICanvas; // Reference to the Multiplayer UI Canvas

    void Start()
    {
        multiplayerUICanvas = GameObject.Find("NetworkCanvas");

        if (multiplayerUICanvas == null)
        {
            Debug.LogError(
                "❌ NetworkCanvas not found in the scene! Make sure it exists and is named correctly."
            );
        }
    }

    void Update()
    {
        // Get movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        MovementInput = new Vector3(moveHorizontal, 0, moveVertical);

        // Detect jump and buffer it for FixedUpdate()
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpBuffered = true;
        }

        SprintInput = Input.GetKey(KeyCode.LeftShift);
        // Toggle Multiplayer UI with Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMultiplayerUI();
        }
    }

    void ToggleMultiplayerUI()
    {
        if (multiplayerUICanvas != null)
        {
            multiplayerUICanvas.SetActive(!multiplayerUICanvas.activeSelf);
        }
    }
}
