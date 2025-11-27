using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The player transform (optional - can be found automatically)

    [Header("Auto-Find Settings")]
    public string playerTag = "Player"; // Tag to search for
    public string playerName = "Player Variant"; // Or search by name
    public bool findByTag = true; // If true, uses tag; if false, uses name

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Camera position relative to player
    public float smoothSpeed = 0.125f; // How smoothly the camera follows (0-1)

    [Header("Optional Settings")]
    public bool lockX = false; // Lock camera movement on X axis
    public bool lockY = false; // Lock camera movement on Y axis
    public bool lockZ = false; // Lock camera movement on Z axis

    void Start()
    {
        // Try to find the player if target is not assigned
        if (target == null)
        {
            FindPlayer();
        }
    }

    void FindPlayer()
    {
        if (findByTag)
        {
            // Find by tag
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                target = playerObj.transform;
                Debug.Log("Camera found player by tag: " + playerObj.name);
            }
            else
            {
                Debug.LogWarning("Could not find player with tag: " + playerTag);
            }
        }
        else
        {
            // Find by name (searches through all objects, including nested ones)
            GameObject playerObj = GameObject.Find(playerName);
            if (playerObj != null)
            {
                target = playerObj.transform;
                Debug.Log("Camera found player by name: " + playerObj.name);
            }
            else
            {
                Debug.LogWarning("Could not find player with name: " + playerName);
            }
        }
    }

    void LateUpdate()
    {
        // If target is still null, try to find it again
        if (target == null)
        {
            FindPlayer();
            return;
        }

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Apply axis locks if needed
        if (lockX) desiredPosition.x = transform.position.x;
        if (lockY) desiredPosition.y = transform.position.y;
        if (lockZ) desiredPosition.z = transform.position.z;

        // Smoothly interpolate between current position and desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update camera position
        transform.position = smoothedPosition;

        // Optional: Make camera look at the target
        // Uncomment the line below if you want the camera to always look at the player
        // transform.LookAt(target);
    }
}