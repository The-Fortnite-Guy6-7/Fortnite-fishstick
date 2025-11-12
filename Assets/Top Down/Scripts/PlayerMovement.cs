using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting; // Removed unused directive
// using UnityEditor; // Removed unused directive (Editor scripts shouldn't be in play mode scripts)
using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityEngine.Serialization; // Removed unused directive
// using Random = UnityEngine.Random; // Removed unused directive

public class PlayerMovement : MonoBehaviour
{
    // A static reference for easy access
    public static PlayerMovement Player { get; private set; } // Use auto-property with private set

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb; // Use SerializeField and standard naming
    [SerializeField] private ProjectileController bulletPrefab; // Use standard naming

    [Header("Stats")]
    public float speed = 5; // Use standard naming

    private Camera mainCamera; // Cache the main camera reference

    private void Awake()
    {
        // Singleton pattern: Ensure only one player instance exists
        if (Player != null && Player != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Player = this;
    }

    void Start()
    {
        // Get components if not assigned in Inspector
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        // Cache the main camera for performance
        mainCamera = Camera.main;

        if (rb == null) Debug.LogError("Rigidbody2D not found on PlayerMovement!");
    }

    void Update()
    {
        HandleMovementInput();
        HandleShooting();
    }

    private void HandleMovementInput()
    {
        if (rb == null) return;

        // Use GetAxisRaw for cleaner input aggregation in top-down movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * speed;
        // Apply velocity directly (Update is okay for this style)
        rb.linearVelocity = movement;
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && bulletPrefab != null && mainCamera != null)
        {
            // Find mouse position in world space
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // Ensure Z is zero for 2D calculations

            // Calculate direction vector
            Vector3 direction = (mouseWorldPos - transform.position).normalized;

            // Calculate angle in degrees
            // Mathf.Atan2 returns radians, convert to degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Spawn the projectile with the calculated rotation
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If I walk into the exit. . .
        if (other.CompareTag("Exit")) // Use CompareTag for performance
        {
            SceneManager.LoadScene("You Win");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // If I walk into a monster or other hazard. . .
        if (other.gameObject.CompareTag("Hazard")) // Use CompareTag for performance
        {
            SceneManager.LoadScene("You Lose");
        }
    }
}
