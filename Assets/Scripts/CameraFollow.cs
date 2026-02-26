using UnityEngine;
using UnityEngine.InputSystem; // Required to read the mouse in the New Input System

public class CameraFollow : MonoBehaviour
{
    [Header("Target Setup")]
    [Tooltip("Drag your Player1 object here.")]
    public Transform target;
    
    [Tooltip("How far the camera stays from the player.")]
    public float distance = 5.0f;
    
    [Tooltip("Offsets the camera so it looks at the player's body/head, not their feet.")]
    public Vector3 targetOffset = new Vector3(0, 1f, 0); 

    [Header("Mouse Controls")]
    public float mouseSensitivity = 0.2f;
    public float minYAngle = -20f; // How low you can look (looking up at player)
    public float maxYAngle = 80f;  // How high you can look (looking down at player)

    private float currentX = 0f;
    private float currentY = 0f;

    void Start()
    {
        // MAGIC TRICK: This locks your mouse to the game screen and hides the cursor!
        // IMPORTANT: Press the ESCAPE key on your keyboard to get your mouse back.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Read the Mouse Movement
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            currentX += mouseDelta.x * mouseSensitivity;
            currentY -= mouseDelta.y * mouseSensitivity; 
        }

        // 2. Clamp the Y angle so the camera doesn't flip upside down
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

        // 3. Calculate the new rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // 4. Calculate the new position (orbiting around the target)
        Vector3 lookAtPosition = target.position + targetOffset;
        Vector3 position = lookAtPosition - (rotation * Vector3.forward * distance);

        // 5. Apply the math to the Camera
        transform.position = position;
        transform.rotation = rotation;
    }
}