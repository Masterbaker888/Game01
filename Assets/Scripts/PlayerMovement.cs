using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Animator animator;
    public Transform mainCamera;

    [Header("Movement Settings")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Gravity & Jumping")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    Vector3 velocity;
    bool isGrounded;

    // Used to track the current animation so it doesn't stutter
    private string currentState;

    void Update()
    {
        // 1. Check if grounded
        isGrounded = controller.isGrounded;
        Debug.Log("Am I on the ground? " + isGrounded); // <--- ADD THIS LINE
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Helps keep the character snapped perfectly to the floor
        }

        // 2. Get Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 3. Movement and Rotation (Fortnite Style)
        if (direction.magnitude >= 0.1f)
        {
            // Always face the exact same direction the camera is looking
            float targetAngle = mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move forward, backward, left, or right based on the keys pressed
            Vector3 moveDir = transform.right * direction.x + transform.forward * direction.z;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            if (isGrounded) 
            {
                ChangeAnimationState("Running");
            }
        }
        else
        {
            if (isGrounded) 
            {
                // Just in case we stop moving but still want to face the camera direction
                float targetAngle = mainCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                
                ChangeAnimationState("Idle");
            }
        }

        // 4. Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Helper function to play animations without making them jitter
    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        
        animator.Play(newState);
        currentState = newState;
    }
}