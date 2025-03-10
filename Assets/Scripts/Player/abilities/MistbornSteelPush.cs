using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections;
using System.Collections.Generic;

// Example Mistborn Power (Placeholder)
public class MistbornSteelPush : Power
{
    public float PushStrength = 5f;
    
    public override void Activate()
    {
        if (!CanActivate()) return;
        Debug.Log("Steel Push activated!");
        SteelPushServerRpc();
    }

    public override bool CanActivate()
    {
        return true; // Add logic for availability
    }

    [ServerRpc]
    private void SteelPushServerRpc()
    {
        Debug.Log("Steel Push executed on server");
        // Apply force logic here
    }
}