using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Abstract base class for all abilities (including melee attacks and Mistborn powers)
public abstract class Power : NetworkBehaviour
{
    public string PowerName;
    public float Cooldown;
    
    public abstract void Activate(); // Core activation method
    public abstract bool CanActivate(); // Check if activation is allowed
}
