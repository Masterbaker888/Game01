using UnityEngine;
// We need this namespace to use the new Unity Input System
using UnityEngine.InputSystem; 

// This line forces Unity to automatically add a CharacterController to the player if it's missing
[RequireComponent(typeof(CharacterController))]
public class Player1Movement : MonoBehaviour
{
    #region Input Actions
    [Header("Input System References")]
    [Tooltip("Drag the Move input action here (Vector2)")]
    [SerializeField] private InputActionReference moveAction;
    
    [Tooltip("Drag the Jump input action here (Button)")]
    [SerializeField] private InputActionReference jumpAction;
    
    [Tooltip("Drag the Sprint input action here (Button)")]
    [SerializeField] private InputActionReference sprintAction;
    
    [Tooltip("Drag the Walk Faster input action here (Button)")]
    [SerializeField] private InputActionReference fastWalkAction;
    
    [Tooltip("Drag the Crouch input action here (Button)")]
    [SerializeField] private InputActionReference crouchAction;
    #endregion

    #region Movement Parameters
    [Header("Movement Speeds")]
    [Tooltip("Base speed when walking normally.")]
    [SerializeField] private float walkSpeed = 5f;
    
    [Tooltip("Speed when the Fast Walk button is held.")]
    [SerializeField] private float fastWalkSpeed = 8f;
    
    [Tooltip("Speed when the Sprint button is held.")]
    [SerializeField] private float sprintSpeed = 12f;
    
    [Tooltip("Speed when crouching.")]
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("Jumping & Gravity")]
    [Tooltip("How high the player can jump.")]
    [SerializeField] private float jumpHeight = 2f;
    
    [Tooltip("The strength of gravity pulling the player down.")]
    [SerializeField] private float gravity = -9.81f;
    
    [Tooltip("How long (in seconds) the player can still jump after walking off a ledge.")]
    [SerializeField] private float coyoteTime = 0.2f;
    
    [Tooltip("Maximum number of jumps allowed (2 = Double Jump).")]
    [SerializeField] private int maxJumps = 2;

    [Header("Crouching Setup")]
    [Tooltip("The height of the player's collider when crouching.")]
    [SerializeField] private float crouchHeight = 1f;
    #endregion

    #region Ground Detection
    [Header("Ground Detection Setup")]
    [Tooltip("An empty GameObject placed at the very bottom of the player's feet.")]
    [SerializeField] private Transform groundCheck;
    
    [Tooltip("The radius of the invisible sphere checking for the ground.")]
    [SerializeField] private float groundDistance = 0.4f;
    
    [Tooltip("The Layer that represents the Ground.")]
    [SerializeField] private LayerMask groundMask;
    #endregion

    #region State Variables
    // These variables track the player's current state behind the scenes
    private CharacterController controller; // Reference to the movement component
    private Vector3 velocity;               // Tracks downward falling speed
    private bool isGrounded;                // Is the player currently touching the ground?
    private float coyoteTimeCounter;        // Timer for coyote time
    private int jumpCount;                  // Tracks how many times we've jumped
    private float originalHeight;           // Stores the standing height of the player
    #endregion

    #region Unity Methods
    // Start runs once when the game begins
    private void Start()
    {
        // Grab the Character Controller component attached to this GameObject
        controller = GetComponent<CharacterController>();
        
        // Save the original height so we can stand back up after crouching
        originalHeight = controller.height;
    }

    // Update runs once every single frame (often 60+ times a second)
    private void Update()
    {
        HandleGroundCheckAndGravity();
        HandleMovement();
        HandleCrouching();
        HandleJumping();
    }
    #endregion

    #region Movement Logic
    private void HandleGroundCheckAndGravity()
    {
        // Creates an invisible sphere at the player's feet. If it touches the groundMask, isGrounded becomes true.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // If we are on the ground and falling, reset our downward velocity so it doesn't build up infinitely
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // A small negative number keeps the player snapped firmly to the floor
        }

        // Apply gravity over time (Gravity accelerates, so we multiply by Time.deltaTime twice mathematically)
        velocity.y += gravity * Time.deltaTime;
        
        // Move the controller downward based on gravity
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        // 1. Read the input from the joystick/keyboard (X is left/right, Y is up/down on the stick)
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // 2. Convert 2D input into 3D movement on a flat plane (X and Z axes)
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        // 3. Determine the current speed based on what buttons are held down
        float currentSpeed = walkSpeed; // Default to walk

        if (crouchAction.action.IsPressed())
        {
            currentSpeed = crouchSpeed;
        }
        else if (sprintAction.action.IsPressed())
        {
            currentSpeed = sprintSpeed;
        }
        else if (fastWalkAction.action.IsPressed())
        {
            currentSpeed = fastWalkSpeed;
        }

        // 4. Move the Character Controller
        // Time.deltaTime ensures the speed is frame-rate independent (same speed on fast/slow computers)
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Optional: Make the player face the direction they are walking
        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection; 
        }
    }

    private void HandleCrouching()
    {
        // If the crouch button is currently being held down...
        if (crouchAction.action.IsPressed())
        {
            controller.height = crouchHeight; // Shrink the collider
        }
        else
        {
            controller.height = originalHeight; // Stand back up
        }
    }
    #endregion

    #region Jumping Logic
    private void HandleJumping()
    {
        // Manage the Coyote Time timer
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reset timer when touching the ground
            jumpCount = 0;                  // Reset jump count
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease timer while falling
        }

        // Check if the jump button was pressed exactly on this frame
        if (jumpAction.action.WasPressedThisFrame())
        {
            // Can we jump? (Either we have coyote time left, OR we haven't used all our double jumps)
            if (coyoteTimeCounter > 0f || jumpCount < maxJumps)
            {
                // Physics formula for calculating the velocity needed to reach a specific jump height
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                
                jumpCount++;               // Add 1 to our jump count
                coyoteTimeCounter = 0f;    // Consume the coyote time so we can't infinitely jump
            }
        }
    }
    #endregion
}