using UnityEngine;
using UnityEngine.InputSystem; // Use new Input System

public class PlayerInput : MonoBehaviour
{
    public Vector3 MovementInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool JumpBuffered { get; private set; }
    private GameObject multiplayerUICanvas;
    private PlayerLoadout playerLoadout;
    private PlayerInputActions inputActions; // New Input Actions

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        // Bind Input Actions
        inputActions.Player.Jump.performed += ctx => JumpBuffered = true;
        inputActions.Player.Sprint.performed += ctx => SprintInput = true;
        inputActions.Player.Sprint.canceled += ctx => SprintInput = false;
        inputActions.Player.ToggleUI.performed += ctx => ToggleMultiplayerUI();
        // inputActions.Player.Power1.performed += ctx => UsePower(0);
        // inputActions.Player.Power2.performed += ctx => UsePower(1);
        inputActions.Player.Power1.performed += ctx =>
        {
            Debug.Log("Power1 Input Received. Calling UsePower(0)");
            UsePower(0);
        };

        inputActions.Player.Power2.performed += ctx =>
        {
            Debug.Log("Power2 Input Received. Calling UsePower(1)");
            UsePower(1);
        };
        inputActions.Player.Power3.performed += ctx =>
        {
            Debug.Log("Power3 Input Received. Calling UsePower(2)");
            UsePower(2);
        };
    }

    void Start()
    {
        multiplayerUICanvas = GameObject.Find("NetworkCanvas");
        playerLoadout = GetComponent<PlayerLoadout>();

        if (multiplayerUICanvas == null)
        {
            Debug.LogError(
                "NetworkCanvas not found in the scene! Make sure it exists and is named correctly."
            );
        }
    }

    void Update()
    {
        Vector2 moveInput = inputActions.Player.Movement.ReadValue<Vector2>();
        MovementInput = new Vector3(moveInput.x, 0, moveInput.y);
    }

    void ToggleMultiplayerUI()
    {
        if (multiplayerUICanvas != null)
        {
            multiplayerUICanvas.SetActive(!multiplayerUICanvas.activeSelf);
        }
    }

    void UsePower(int index)
    {
        Debug.Log("trying to UsePower");
        if (playerLoadout != null)
        {
            playerLoadout.UsePower(index);
        }
    }

    public void ResetJumpBuffer()
    {
        JumpBuffered = false;
    }

    void OnEnable() => inputActions.Player.Enable();

    void OnDisable() => inputActions.Player.Disable();
}
