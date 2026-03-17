using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform mainCamera;
    public Animator animator;

    [Header("Movement Settings")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Jump & Gravity")]
    public float gravity = -15f;
    public float jumpHeight = 9f;
    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it when the game starts
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- SHIFT LOCK TOGGLE ---
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // Unlock and show the mouse
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Lock and hide the mouse
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // 1. Ground Check (Built-in Character Controller method)
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // 2. Get Player Input
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float vertical = Input.GetAxisRaw("Vertical");     
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 3. Movement and Rotation
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = transform.right * direction.x + transform.forward * direction.z;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        else
        {
            float targetAngle = mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        // 4. Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5. Animation
        animator.SetFloat("VelocityX", horizontal);
        animator.SetFloat("VelocityZ", vertical);
        
        // --- NEW: Tell the animator if we are on the ground or in the air! ---
        animator.SetBool("IsGrounded", isGrounded);
    }
}