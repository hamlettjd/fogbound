using UnityEngine;

// Attach this script to any GameObject that should be considered metal.
// The weight property lets you adjust the effective mass for push interactions.
public class MetalComponent : MonoBehaviour
{
    public float weight = 1f; // Adjust in the inspector to represent the object's material and size.
    public bool isImmovable = false; // Indicates whether this object should be immovable.

    // Reference to the dynamically created metal proxy, if any.
    private GameObject metalProxy;

    void Awake()
    {
        // Check if this object is on the "Ground" layer.
        if (gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isImmovable = true;
            CreateMetalProxy();
        }
        else
        {
            isImmovable = false;
        }
    }

    private void CreateMetalProxy()
    {
        // Create a new child GameObject to act as the metal proxy.
        metalProxy = new GameObject("MetalProxy");
        metalProxy.transform.SetParent(transform);
        // Align the proxy's transform with the parent.
        metalProxy.transform.localPosition = Vector3.zero;
        metalProxy.transform.localRotation = Quaternion.identity;
        metalProxy.transform.localScale = Vector3.one;

        // Set the proxy's layer to "Metal" so that it is detected by the Mistborn ability.
        metalProxy.layer = LayerMask.NameToLayer("Metal");

        // Add a SphereCollider and set it as a trigger for detection.
        SphereCollider sc = metalProxy.AddComponent<SphereCollider>();
        sc.isTrigger = true;

        // Use the parent's MeshRenderer bounds to determine a reasonable radius.
        MeshRenderer rend = GetComponent<MeshRenderer>();
        if (rend != null)
        {
            // Use the magnitude of the bounds' extents as an approximation.
            float calculatedRadius = rend.bounds.extents.magnitude;
            sc.radius = calculatedRadius;
            Debug.Log(
                $"[MetalComponent] {gameObject.name} - Calculated metal proxy radius: {calculatedRadius}"
            );
        }
        else
        {
            sc.radius = 0.5f; // Fallback value.
            Debug.Log(
                $"[MetalComponent] {gameObject.name} - No MeshRenderer found. Using default proxy radius: 0.5"
            );
        }

        // IMPORTANT: Add a MetalComponent to the proxy so that the push logic can retrieve it.
        // This won't trigger another proxy creation because the proxy is on the Metal layer.
        MetalComponent proxyMetalComp = metalProxy.AddComponent<MetalComponent>();
        // Copy the parent's weight to the proxy (or adjust as needed).
        proxyMetalComp.weight = this.weight;
        // Ensure the proxy isn't marked as immovable.
        proxyMetalComp.isImmovable = false;
    }
}
