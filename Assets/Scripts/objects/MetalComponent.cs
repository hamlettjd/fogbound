using UnityEngine;

// Attach this script to any GameObject that should be considered metal.
// The weight property lets you adjust the effective mass for push interactions.
public class MetalComponent : MonoBehaviour
{
    public float weight = 1f; // Adjust in the inspector to represent the object's material and size.
}
