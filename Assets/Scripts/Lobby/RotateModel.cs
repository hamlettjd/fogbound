using UnityEngine;

public class RotateModel : MonoBehaviour
{
    public float rotationSpeed = 10f; // degrees per second

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
