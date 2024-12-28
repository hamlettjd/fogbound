using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector3 MovementInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool SprintInput { get; private set; }

    void Update()
    {
        // Get movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        MovementInput = new Vector3(moveHorizontal, 0, moveVertical);

        // Get jump and sprint inputs
        JumpInput = Input.GetKeyDown(KeyCode.Space);
        SprintInput = Input.GetKey(KeyCode.LeftShift);
    }
}
