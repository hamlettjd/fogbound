using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MistbornBurnVisuals : Power
{
    // The radius within which metal objects are detected.
    public float VisualRadius = 5f;

    // Layer mask to filter metal objects.
    public LayerMask MetalLayerMask;

    // Prefab of the blue line (should have a LineRenderer component).
    public GameObject BlueLinePrefab;

    // Transform representing the chest position. Assign this in the Inspector.
    public Transform ChestTransform;

    // Padding at the start and end of the line, so the line doesn't go right into the character or target.
    public float StartPadding = 0.5f;
    public float EndPadding = 0.3f;

    // Parameters for line widths.
    public float EndpointWidth = 0.03f;
    public float MiddleWidth = 0.02f;

    // New parameters for the jagged effect.
    // Total number of points along the line (including start and end).
    public int PointCount = 5;

    // Maximum offset for intermediate points.
    public float MaxOffset = 0.2f;

    // Keeps track of active lines for each metal collider.
    private Dictionary<Collider, LineRenderer> activeLines =
        new Dictionary<Collider, LineRenderer>();

    // Stores random offset factors for each active line (for intermediate points).
    private Dictionary<Collider, float[]> jaggedOffsets = new Dictionary<Collider, float[]>();

    // Toggle flag: are burn visuals active?
    private bool isActive = false;

    // Toggle the burn visuals on or off.
    public override void Activate()
    {
        isActive = !isActive;
        if (!isActive)
        {
            ClearActiveLines();
        }
    }

    public override bool CanActivate()
    {
        return true;
    }

    // Clears and destroys all active line objects.
    private void ClearActiveLines()
    {
        foreach (var kvp in activeLines)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value.gameObject);
            }
        }
        activeLines.Clear();
        jaggedOffsets.Clear();
    }

    private void Update()
    {
        // Only update visuals when the burn power is active.
        if (!isActive)
        {
            return;
        }

        // Detect metal objects within the VisualRadius.
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            VisualRadius,
            MetalLayerMask
        );
        HashSet<Collider> detectedColliders = new HashSet<Collider>(hitColliders);

        // Remove lines for objects that are no longer in range.
        List<Collider> toRemove = new List<Collider>();
        foreach (var kvp in activeLines)
        {
            if (!detectedColliders.Contains(kvp.Key))
            {
                Destroy(kvp.Value.gameObject);
                toRemove.Add(kvp.Key);
            }
        }
        foreach (Collider col in toRemove)
        {
            activeLines.Remove(col);
            jaggedOffsets.Remove(col);
        }

        // For each detected metal object, create or update the line.
        foreach (Collider col in hitColliders)
        {
            LineRenderer lr;
            if (!activeLines.TryGetValue(col, out lr))
            {
                GameObject lineObj = Instantiate(BlueLinePrefab);
                lr = lineObj.GetComponent<LineRenderer>();
                lr.positionCount = PointCount;

                // Set a width curve that makes endpoints thicker than the middle.
                AnimationCurve widthCurve = new AnimationCurve();
                widthCurve.AddKey(0f, EndpointWidth);
                widthCurve.AddKey(0.5f, MiddleWidth);
                widthCurve.AddKey(1f, EndpointWidth);
                lr.widthCurve = widthCurve;

                activeLines.Add(col, lr);

                // Generate random offsets for intermediate points (not including start and end).
                float[] offsets = new float[PointCount - 2];
                for (int i = 0; i < offsets.Length; i++)
                {
                    offsets[i] = Random.Range(-1f, 1f);
                }
                jaggedOffsets.Add(col, offsets);
            }

            // Calculate adjusted endpoints.
            Vector3 rawStart = ChestTransform.position;
            Vector3 rawEnd = col.transform.position;
            Vector3 direction = (rawEnd - rawStart).normalized;
            Vector3 adjustedStart = rawStart + direction * StartPadding;
            Vector3 adjustedEnd = rawEnd - direction * EndPadding;

            // Determine a perpendicular direction for offsetting intermediate points.
            Vector3 baseDir = adjustedEnd - adjustedStart;
            Vector3 arbitrary = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(baseDir.normalized, arbitrary)) > 0.9f)
            {
                arbitrary = Vector3.right;
            }
            Vector3 perp = Vector3.Cross(baseDir, arbitrary).normalized;

            // Update each point along the line.
            for (int i = 0; i < PointCount; i++)
            {
                float t = (float)i / (PointCount - 1); // Interpolation factor from 0 to 1.
                Vector3 pos = Vector3.Lerp(adjustedStart, adjustedEnd, t);
                // For intermediate points, add a perpendicular offset.
                if (i != 0 && i != PointCount - 1)
                {
                    float offsetFactor = jaggedOffsets[col][i - 1];
                    pos += perp * offsetFactor * MaxOffset;
                }
                lr.SetPosition(i, pos);
            }
        }
    }
}
