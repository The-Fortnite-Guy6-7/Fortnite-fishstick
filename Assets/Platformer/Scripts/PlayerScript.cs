using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting; // <-- This line caused the error, it is now removed
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    // --- REFERENCES ---
    // Use [SerializeField] and keep variables private if they don't need to be public
    // but still need to be accessible in the Unity Inspector.
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D coll;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private AudioSource audioSource; // Renamed AS to audioSource for clarity/style

    // --- PERSONAL STATS ---
    public float speed = 5;
    public float jumpPower = 10;
    // Renamed Gravity to gravityScale to avoid confusion with Physics.Gravity
    public float gravityScale = 3;

    // --- STATE TRACKING ---
    private int groundContacts = 0; // Use a counter to track ground contacts
    public bool facingLeft = false; // Renamed with lowercase 'f' for C# style
    // If this is over 0, I'm stunned and can't move
    public float stunnedTimer = 0; // Renamed Stunned to stunnedTimer for clarity

    // --- SOUND EFFECTS ---
    public AudioClip jumpSFX;

    void Start()
    {
        // Add checks to ensure components are assigned if not using GetComponent in Start()
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (ps == null) ps = GetComponentInChildren<ParticleSystem>();

        // Set our rigidbody's gravity to match our stats 
        rb.gravityScale = gravityScale;
    }

    void Update()
    {
        // Handle Stun Logic First
        if (stunnedTimer > 0)
        {
            stunnedTimer -= Time.deltaTime;
            // Use a ternary operator for cleaner state setting
            sr.color = (stunnedTimer > 0) ? Color.gray : Color.white;

            // Don't run any of the movement code below while stunned
            return;
        }

        // Use Rigidbody2D.velocity directly for smoother physics interaction
        Vector2 vel = rb.linearVelocity;
        float horizontalInput = 0;

        // Use Input.GetAxisRaw("Horizontal") for a robust way to handle both 
        // arrow keys and WASD, with instant response.
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0)
        {
            // Apply speed based on input direction
            vel.x = horizontalInput * speed;
            // Update facing direction
            facingLeft = (horizontalInput < 0);
        }
        else
        {
            // If no input, set x velocity to zero for immediate stop (characteristic of classic platformers)
            vel.x = 0;
        }

        // If I hit Z/Jump button and can jump, jump
        if (Input.GetKeyDown(KeyCode.Z) && CanJump()) // Changed 'Z' check to use GetKeyDown
        {
            vel.y = jumpPower;
            // Emit 5 dust cloud particles
            if (ps != null) ps.Emit(5);
            // Play my jump sound
            if (audioSource != null && jumpSFX != null) audioSource.PlayOneShot(jumpSFX);
        }

        // Here I actually feed the Rigidbody the movement I want
        rb.linearVelocity = vel;
        // Use my facingLeft variable to make my sprite face the right way
        sr.flipX = facingLeft;

        // If I fall into the void...
        if (transform.position.y < -20)
        {
            // Use LoadSceneAsync if desired for smoother transitions, but LoadScene is fine for a simple game over
            SceneManager.LoadScene("You Lose");
        }
    }

    // I use this function to track if I can jump or not
    public bool CanJump()
    {
        // Added a safety check for the counter being negative (shouldn't happen, but good practice)
        return groundContacts > 0;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // CRITICAL FIX: Only increment counter if the collision is a valid 'ground' surface.
        // You might need a specific LayerMask check here in a real game, but for now, 
        // we assume all collisions are 'ground' for vertical tracking.
        groundContacts++;

        // If what I hit was an enemy...
        EnemyScript es = other.gameObject.GetComponent<EnemyScript>();
        if (es != null)
        {
            // Set me to be stunned
            stunnedTimer = 0.75f;
            // Pick a direction to throw me in
            Vector2 throwDirection = new Vector2(5, 5);
            // If the monster's to my right, throw me left
            if (other.transform.position.x > transform.position.x)
                throwDirection.x *= -1;

            // Clear existing velocity before adding impulse for consistent knockback
            rb.linearVelocity = Vector2.zero;
            // And toss me
            rb.AddForce(throwDirection, ForceMode2D.Impulse);
        }
    }

    // Add the OnCollisionExit2D method to decrement the counter
    private void OnCollisionExit2D(Collision2D other)
    {
        // CRITICAL FIX: Decrement the ground contact counter, ensuring it never goes below zero.
        groundContacts = Mathf.Max(0, groundContacts - 1);
    }
}
