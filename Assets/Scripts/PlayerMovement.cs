using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public CharacterController controller;
    public Transform mainCamera;

    public float speed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -15f; 

    private Vector3 velocity;
    private string currentState = "Idle";

    void Update()
    {
        // 1. Are we touching the floor?
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keeps her snapped to the ground
        }

        // 2. Read WASD keys
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

   // 3. Running and Idling
        if (direction.magnitude >= 0.1f)
        {
            // Move based on where the camera is looking
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            
            // ---> ADD THIS LINE: Makes her physically rotate to face the direction she is running <---
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            if (isGrounded && velocity.y <= 0) 
            {
                ChangeAnimationState("Running");
            }
        }
        else
        {
            // ONLY idle if we are on the ground AND not currently flying upwards from a jump
            if (isGrounded && velocity.y <= 0) 
            {
                ChangeAnimationState("Idle");
            }
        }

        // 4. Jumping (Press Spacebar)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            ChangeAnimationState("Jumping");
        }

        // 5. Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Direct State Switcher
    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return; 
        animator.CrossFade(newState, 0.1f);
        currentState = newState;
    }
}