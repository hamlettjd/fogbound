using UnityEngine;
using Unity.Netcode;

public class MistbornIronPullOmni : Power
{
    public float PullStrength = 5f;
    public float PullRadius = 5f;

    // Within this distance, the pull strength is at full power.
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
            IronPullServerRpc();
        }
        else
        {
            Debug.Log(
                $"[MistbornIronPull] Ignoring activation because client {NetworkManager.LocalClientId} is not the owner."
            );
        }
    }

    public override bool CanActivate()
    {
        return true; // Add additional logic for cooldowns or resource costs as needed.
    }

    [ServerRpc]
    private void IronPullServerRpc()
    {
        Debug.Log("Iron Pull executed on server");

        // The Allomancer's "center of self" is assumed to be transform.position.
        Vector3 center = transform.position;

        // Get the Allomancer's Rigidbody (default mass = 1 if none found).
        Rigidbody allomancerRb = GetComponent<Rigidbody>();
        float allomancerMass = (allomancerRb != null) ? allomancerRb.mass : 1f;
        Debug.Log($"Allomancer mass: {allomancerMass}");

        // Find nearby colliders using the specified layer mask.
        Collider[] hitColliders = Physics.OverlapSphere(center, PullRadius, MetalLayerMask);
        Debug.Log($"Found {hitColliders.Length} colliders within PullRadius {PullRadius}");

        foreach (Collider hit in hitColliders)
        {
            // Use component-based detection to confirm the object is metal.
            MetalComponent metalComp = hit.gameObject.GetComponent<MetalComponent>();
            if (metalComp != null)
            {
                Debug.Log(
                    $"Processing metal object: {hit.gameObject.name} with weight {metalComp.weight}"
                );
                ApplyPullForce(hit, center, allomancerMass, allomancerRb, metalComp);
            }
        }
    }

    private void ApplyPullForce(
        Collider target,
        Vector3 center,
        float allomancerMass,
        Rigidbody allomancerRb,
        MetalComponent metalComp
    )
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        // Compute the direction vector from the Allomancer to the target.
        // (In push this is used directly; here we'll invert it as needed.)
        Vector3 direction = (target.transform.position - center).normalized;
        Debug.Log($"Direction from allomancer to target ({target.gameObject.name}): {direction}");

        // Determine the distance from the Allomancer's center to the target.
        float distance = Vector3.Distance(center, target.transform.position);
        Debug.Log($"Distance to target ({target.gameObject.name}): {distance}");

        // Calculate a distance factor.
        // If the target is within FullForceDistance, factor is 1.
        // Beyond that, it linearly decreases to 0 at PullRadius.
        float distanceFactor = 1f;
        if (distance > FullForceDistance)
        {
            distanceFactor = Mathf.Clamp01(
                1f - (distance - FullForceDistance) / (PullRadius - FullForceDistance)
            );
        }
        Debug.Log($"Distance factor for target ({target.gameObject.name}): {distanceFactor}");

        // Check if the target is on the "Ground" layer (assumed immovable).
        bool immovable = (target.gameObject.layer == LayerMask.NameToLayer("Ground"));

        if (immovable)
        {
            Debug.Log(
                $"Target {target.gameObject.name} is immovable. Applying full pull force to the Allomancer."
            );
            // When the target is immovable, the Allomancer is pulled toward the object.
            allomancerRb.AddForce(direction * PullStrength * distanceFactor, ForceMode.Impulse);
            return;
        }

        // For movable targets, calculate force distribution based on relative masses.
        float targetWeight = metalComp.weight;
        float totalMass = allomancerMass + targetWeight;
        Debug.Log(
            $"Target {target.gameObject.name} weight: {targetWeight}, Total effective mass: {totalMass}"
        );

        // For pulling:
        // - The target receives a force (pull) proportional to the Allomancer's mass,
        //   which is applied towards the Allomancer (i.e. opposite to 'direction').
        // - The Allomancer receives a force (pull) proportional to the target's weight,
        //   which is applied towards the target (i.e. in 'direction').
        float forceOnTarget = PullStrength * (allomancerMass / totalMass) * distanceFactor;
        float forceOnAllomancer = PullStrength * (targetWeight / totalMass) * distanceFactor;
        Debug.Log(
            $"Force on target ({target.gameObject.name}): {forceOnTarget}, Force on Allomancer: {forceOnAllomancer}"
        );

        // Apply pull force to the target (pulling it toward the Allomancer).
        if (targetRb != null)
        {
            targetRb.AddForce(-direction * forceOnTarget, ForceMode.Impulse);
        }

        // Apply pull force to the Allomancer (pulling them toward the target).
        allomancerRb.AddForce(direction * forceOnAllomancer, ForceMode.Impulse);
    }
}
