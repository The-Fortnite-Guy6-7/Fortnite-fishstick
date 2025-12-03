using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    //The camera is inside the player
    public Camera Eyes;
    public Rigidbody RB;
    public Projectile3DController ProjectilePrefab;
    public Transform Hands; // Added this back in from the previous answer

    //Character stats
    public float MouseSensitivity = 3;
    public float WalkSpeed = 10;
    public float JumpPower = 7;

    //A list of all the solid objects I'm currently touching
    public List<GameObject> Floors;

    // Variables to store current movement input
    private Vector3 _moveDirection = Vector3.zero;

    void Start()
    {
        //Turn off my mouse and lock it to center screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle rotation (Input is best checked in Update)
        float xRot = Input.GetAxis("Mouse X") * MouseSensitivity;
        transform.Rotate(0, xRot, 0);
        float yRot = -Input.GetAxis("Mouse Y") * MouseSensitivity;
        Eyes.transform.Rotate(yRot, 0, 0);

        // Handle movement input (store direction for FixedUpdate)
        _moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) _moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) _moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A)) _moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D)) _moveDirection += transform.right;

        // Normalize input and apply speed
        _moveDirection = _moveDirection.normalized * WalkSpeed;

        // Handle Jump Input
        if (JumpPower > 0 && Input.GetKeyDown(KeyCode.Space) && OnGround())
        {
            // Apply instant vertical velocity change for the jump itself
            RB.linearVelocity = new Vector3(RB.linearVelocity.x, JumpPower, RB.linearVelocity.z);
        }

        // Handle shooting (Input is best checked in Update)
        if (Input.GetMouseButtonDown(0) && Hands != null)
        {
            Instantiate(ProjectilePrefab, Hands.transform.position, Hands.transform.rotation);
        }
    }

    // Use FixedUpdate for applying physics forces/velocity to the Rigidbody
    void FixedUpdate()
    {
        // Apply calculated horizontal movement, keeping existing Y velocity (gravity will handle this)
        Vector3 newVelocity = new Vector3(_moveDirection.x, RB.linearVelocity.y, _moveDirection.z);
        RB.linearVelocity = newVelocity;
    }

    // ... (OnGround, OnCollisionEnter, OnCollisionExit methods remain the same) ...
    public bool OnGround()
    {
        return Floors.Count > 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!Floors.Contains(other.gameObject)) Floors.Add(other.gameObject);
    }

    private void OnCollisionExit(Collision other)
    {
        Floors.Remove(other.gameObject);
    }
}
