using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections;
using System.Collections.Generic;

// Player Loadout: Determines which abilities a character has and handles input
public class PlayerLoadout : NetworkBehaviour
{
    public List<Power> AvailablePowers = new List<Power>();
    private PlayerInput playerInput;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
        // commenting out so that it compiles
           // playerInput.actions["Power1"].performed += ctx => UsePower(0);
           // playerInput.actions["Power2"].performed += ctx => UsePower(1);
        }
    }

    private void Start()
    {
        // Load powers dynamically (assign via Inspector or manually here)
        // Example: Predefined character-specific powers can be set here
        AvailablePowers.Add(gameObject.AddComponent<MeleeAttack>());
        AvailablePowers.Add(gameObject.AddComponent<MistbornSteelPush>());
    }
    
    public void UsePower(int index)
    {
        if (index < AvailablePowers.Count)
        {
            AvailablePowers[index].Activate();
        }
    }
}