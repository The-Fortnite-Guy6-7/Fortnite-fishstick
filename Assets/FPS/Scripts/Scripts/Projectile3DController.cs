using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Make sure this using statement matches the namespace defined in NPCHealth.cs
using Assets.FPS.Scripts;

public class Projectile3DController : MonoBehaviour
{
    //My components
    public Rigidbody RB;

    //How fast do I fly?
    public float Speed = 30;
    //How hard do I knockback things I hit?
    public float Knockback = 10;
    // Define how much damage this projectile deals
    public int DamageToDeal = 10;

    void Start()
    {
        //When I spawn, I fly straight forwards at my Speed
        RB.linearVelocity = transform.forward * Speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        // --- ADDED: Damage Logic ---
        // Try to get the NPCHealth component from the object we hit
        NPCHealth healthComponent = other.gameObject.GetComponent<NPCHealth>();

        if (healthComponent != null)
        {
            // If the component exists, call its TakeDamage method
            healthComponent.TakeDamage(DamageToDeal);
        }
        // ---------------------------

        //If I hit something with a rigidbody. . .
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            //I push them in the direction I'm flying with a power equal to my Knockback stat
            rb.AddForce(RB.linearVelocity.normalized * Knockback, ForceMode.Impulse);
        }

        //If I hit anything, I despawn
        Destroy(gameObject);
    }
}
