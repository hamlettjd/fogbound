using UnityEngine;

public class ColliderSizeDebugger : MonoBehaviour
{
    void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Debug.Log(
                $"Collider size magnitude of {gameObject.name}: {collider.bounds.size.magnitude}"
            );
        }
        else
        {
            Debug.Log($"No collider found on {gameObject.name}");
        }
    }
}
