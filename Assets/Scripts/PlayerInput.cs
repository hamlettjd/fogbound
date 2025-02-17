using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector3 MovementInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool JumpBuffered { get; set; }

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
    }
}
