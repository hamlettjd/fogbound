using UnityEngine;
using Unity.Netcode;

public class MistbornSteelPushOmni : Power
{
    public float PushStrength = 5f;
    public float PushRadius = 5f;

    // Within this distance, the push strength is at full power.
    public float FullForceDistance = 2f;

    // LayerMask to filter which colliders to consider.
    public LayerMask MetalLayerMask;

    public override void Activate()
    {
        if (!CanActivate())
        {
            return;
        }

        if (IsOwner) // Ensure only the owning client makes the RPC call
        {
            SteelPushServerRpc();
        }
        else
        {
            Debug.Log(
                $"[MistbornSteelPush] Ignoring activation because client {NetworkManager.LocalClientId} is not the owner."
            );
        }
    }

    public override bool CanActivate()
    {
        return true; // Add additional logic for cooldowns or resource costs as needed.
    }

    [ServerRpc]
    private void SteelPushServerRpc()
    {
        Debug.Log("Steel Push executed on server");

        // The Allomancer's "center of self" is assumed to be transform.position.
        Vector3 center = transform.position;

        // Get the Allomancer's Rigidbody (default mass = 1 if none found).
        Rigidbody allomancerRb = GetComponent<Rigidbody>();
        float allomancerMass = (allomancerRb != null) ? allomancerRb.mass : 1f;
        Debug.Log($"Allomancer mass: {allomancerMass}");

        // Find nearby colliders using the specified layer mask.
        Collider[] hitColliders = Physics.OverlapSphere(center, PushRadius, MetalLayerMask);
        Debug.Log($"Found {hitColliders.Length} colliders within PushRadius {PushRadius}");

        foreach (Collider hit in hitColliders)
        {
            // Use component-based detection to confirm the object is metal.
            MetalComponent metalComp = hit.gameObject.GetComponent<MetalComponent>();
            if (metalComp != null)
            {
                Debug.Log(
                    $"Processing metal object: {hit.gameObject.name} with weight {metalComp.weight}"
                );
                ApplyPushForce(hit, center, allomancerMass, allomancerRb, metalComp);
            }
        }
    }

    private void ApplyPushForce(
        Collider target,
        Vector3 center,
        float allomancerMass,
        Rigidbody allomancerRb,
        MetalComponent metalComp
    )
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 direction = (target.transform.position - center).normalized;
        Debug.Log($"Direction from allomancer to target ({target.gameObject.name}): {direction}");

        // Determine the distance from the Allomancer's center to the target.
        float distance = Vector3.Distance(center, target.transform.position);
        Debug.Log($"Distance to target ({target.gameObject.name}): {distance}");

        // Calculate a distance factor.
        // If the target is within FullForceDistance, factor is 1.
        // Beyond that, it linearly decreases to 0 at PushRadius.
        float distanceFactor = 1f;
        if (distance > FullForceDistance)
        {
            distanceFactor = Mathf.Clamp01(
                1f - (distance - FullForceDistance) / (PushRadius - FullForceDistance)
            );
        }
        Debug.Log($"Distance factor for target ({target.gameObject.name}): {distanceFactor}");

        // Check if the target is on the "Ground" layer (assumed immovable).
        bool immovable = (target.gameObject.layer == LayerMask.NameToLayer("Ground"));

        if (immovable)
        {
            Debug.Log(
                $"Target {target.gameObject.name} is immovable. Applying full (scaled) recoil force to the Allomancer."
            );
            allomancerRb.AddForce(-direction * PushStrength * distanceFactor, ForceMode.Impulse);
            return;
        }

        // For movable targets, calculate force distribution based on relative masses.
        float targetWeight = metalComp.weight;
        float totalMass = allomancerMass + targetWeight;
        Debug.Log(
            $"Target {target.gameObject.name} weight: {targetWeight}, Total effective mass: {totalMass}"
        );

        // The target receives force proportional to the Allomancer's mass,
        // and the Allomancer gets recoil proportional to the target's weight.
        float forceOnTarget = PushStrength * (allomancerMass / totalMass) * distanceFactor;
        float forceOnAllomancer = PushStrength * (targetWeight / totalMass) * distanceFactor;
        Debug.Log(
            $"Force on target ({target.gameObject.name}): {forceOnTarget}, Force on Allomancer: {forceOnAllomancer}"
        );

        // Apply recoil force to the Allomancer (opposite to push direction).
        allomancerRb.AddForce(-direction * forceOnAllomancer, ForceMode.Impulse);

        // Apply force to the target.
        if (targetRb != null)
        {
            targetRb.AddForce(direction * forceOnTarget, ForceMode.Impulse);
        }
    }
}
