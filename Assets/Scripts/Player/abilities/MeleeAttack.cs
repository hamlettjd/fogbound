using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections;
using System.Collections.Generic;

// Example of a Melee Attack Power
public class MeleeAttack : Power
{
    public float Damage = 10f;
    public float AttackRange = 2f;

    public override void Activate()
    {
        if (!CanActivate())
            return;

        Debug.Log("Melee attack activated!");
        // Add attack logic here (raycast for hit detection, animation trigger, etc.)
        MeleeAttackServerRpc();
    }

    public override bool CanActivate()
    {
        return true; // Add cooldown and other conditions
    }

    [ServerRpc]
    private void MeleeAttackServerRpc()
    {
        Debug.Log("Melee attack executed on server");
        // Apply damage, sync animations, etc.
    }
}
