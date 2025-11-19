using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    // === Existing FPS Variables ===
    public Camera Eyes; // The FPS Camera inside the player
    public Rigidbody RB;
    public Projectile3DController ProjectilePrefab;

    // Character stats
    public float MouseSensitivity = 3;
    public float WalkSpeed = 10;
    public float JumpPower = 7;

    // A list of all the solid objects I'm currently touching
    public List<GameObject> Floors;

    // === NEW TPS Variables ===
    // Assign these in the Inspector
    public Camera ThirdPersonCam;
    // You need a GameObject that holds your full character model (e.g., "PlayerModel")
    public GameObject CharacterModel;
    private bool isFPSActive = true;


    void Start()
    {
        // Turn off my mouse and lock it to center screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start in FPS mode
        Eyes.gameObject.SetActive(true);
        ThirdPersonCam.gameObject.SetActive(false);
        // Hide the character model when in FPS view (assuming FPS arms/gun are separate children)
        CharacterModel.SetActive(false);
    }


    void Update()
    {
        HandleCameraMovement();
        HandlePlayerMovement();
        HandleShooting();

        if (Input.GetKeyDown(KeyCode.V)) // Press 'V' to toggle view
        {
            ToggleView();
        }
    }

    private void HandleCameraMovement()
    {
        // If my mouse goes left/right my body moves left/right
        float xRot = Input.GetAxis("Mouse X") * MouseSensitivity;
        transform.Rotate(0, xRot, 0);

        // If my mouse goes up/down my aim (but not body) go up/down
        float yRot = -Input.GetAxis("Mouse Y") * MouseSensitivity;

        if (isFPSActive)
        {
            // Only rotate the FPS camera up/down
            Eyes.transform.Rotate(yRot, 0, 0);
        }
        else
        {
            // The TPS camera script handles its own rotation, but we still rotate the parent transform
            // This ensures the character body turns correctly in TPS mode
        }
    }

    private void HandlePlayerMovement()
    {
        //Movement code (mostly unchanged)
        if (WalkSpeed > 0)
        {
            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                move += transform.forward;
            if (Input.GetKey(KeyCode.S))
                move -= transform.forward;
            if (Input.GetKey(KeyCode.A))
                move -= transform.right;
            if (Input.GetKey(KeyCode.D))
                move += transform.right;

            move = move.normalized * WalkSpeed;

            if (JumpPower > 0 && Input.GetKeyDown(KeyCode.Space) && OnGround())
                move.y = JumpPower;
            else
                move.y = RB.linearVelocity.y;

            RB.linearVelocity = move;
        }
    }

    private void HandleShooting()
    {
        // If I click. . .
        if (Input.GetMouseButtonDown(0))
        {
            // Spawn a projectile from whichever camera is currently active
            Camera activeCam = isFPSActive ? Eyes : ThirdPersonCam;

            // Spawn a projectile right in front of the active camera
            Instantiate(ProjectilePrefab, activeCam.transform.position + activeCam.transform.forward,
                activeCam.transform.rotation);
        }
    }

    private void ToggleView()
    {
        isFPSActive = !isFPSActive;

        Eyes.gameObject.SetActive(isFPSActive);
        ThirdPersonCam.gameObject.SetActive(!isFPSActive);

        // Toggle visibility of the character model
        CharacterModel.SetActive(!isFPSActive);
    }

    // I count as being on the ground if I'm touching at least one solid object
    public bool OnGround()
    {
        return Floors.Count > 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!Floors.Contains(other.gameObject))
            Floors.Add(other.gameObject);
    }

    private void OnCollisionExit(Collision other)
    {
        Floors.Remove(other.gameObject);
    }
}
