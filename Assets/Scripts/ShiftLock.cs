using UnityEngine;

public class ShiftLock : MonoBehaviour
{
    public Transform mainCamera;
    public bool isShiftLocked = false;

    void Update()
    {
        // 1. Toggle the lock on and off when you press Left Shift
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isShiftLocked = !isShiftLocked;
        }

        // 2. If locked, force the character to face exactly where the camera is looking
        if (isShiftLocked)
        {
            // Get the camera's forward direction
            Vector3 cameraForward = mainCamera.forward;
            
            // Keep it flat so your character doesn't tilt up into the sky or down into the dirt
            cameraForward.y = 0; 
            
            // Instantly snap the character to face that direction
            transform.forward = cameraForward;
        }
    }
}