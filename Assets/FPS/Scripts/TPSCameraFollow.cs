using UnityEngine;

public class TPSCameraFollow : MonoBehaviour
{
    // Assign the Player's parent GameObject (the one with the Rigidbody/Controller) here
    public Transform target;
    public Vector3 offset; // e.g., (0, 1.5, -4)
    public float smoothSpeed = 10f;

    void Start()
    {
        // Ensure the camera starts at the correct position relative to the player
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    // Use LateUpdate for camera movement for smoother results after player movement
    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position
            Vector3 desiredPosition = target.position + target.rotation * offset;

            // Smoothly move the camera to the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            // Make the camera always look at the player's position
            transform.LookAt(target.position + new Vector3(0, 1.5f, 0)); // Look slightly up at the player's center
        }
    }
}
