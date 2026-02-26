using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] [Tooltip("Base speed when walking normally.")]
    private float walkSpeed = 5f;
    
    [SerializeField] [Tooltip("Speed when the Sprint button is held.")]
    private float sprintSpeed = 10f;

    [SerializeField] [Tooltip("How fast the character rotates.")]
    private float rotationSpeed = 700f;

    [Header("Jumping & Gravity")]
    [SerializeField] [Tooltip("How high the player can jump.")]
    private float jumpHeight = 2.5f;
    
    [SerializeField] [Tooltip("Gravity strength.")]
    private float gravity = -19.62f;
    
    [SerializeField] [Tooltip("Buffer time (seconds) you can jump after falling off a ledge.")]
    private float coyoteTime = 0.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool isSprinting;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private bool canDoubleJump;

    // NEW: A variable to store our Main Camera
    private Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Automatically find the Main Camera in the scene so we don't have to drag and drop it!
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        CheckGround();
        MovePlayer();
        ApplyGravity();
    }

    #region Input Methods
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (coyoteTimeCounter > 0)
            {
                PerformJump();
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                PerformJump();
                canDoubleJump = false;
            }
        }
    }
    #endregion

    #region Core Logic
    private void CheckGround()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            if (velocity.y < 0) 
            {
                velocity.y = -2f; 
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; 
        }
    }

    private void MovePlayer()
    {
        // If we don't have a camera, stop right here
        if (cameraTransform == null) return;

        // 1. Get the exact directions the camera is facing
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // 2. Flatten those directions so looking down doesn't push the player into the floor
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // 3. Calculate our new movement direction relative to the camera
        Vector3 move = (forward * moveInput.y) + (right * moveInput.x);

        // 4. Move the player
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 5. Rotate the player to instantly face the new walking direction
        if (move != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void PerformJump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        coyoteTimeCounter = 0; 
    }
    #endregion
}