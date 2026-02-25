using UnityEngine;
using UnityEngine.InputSystem; // Required for the New Input System

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables: Movement Settings
    [Header("Movement Speeds")]
    [SerializeField] [Tooltip("How fast the player walks.")]
    private float walkSpeed = 5f;

    [SerializeField] [Tooltip("How fast the player runs when sprinting.")]
    private float sprintSpeed = 8f;

    [SerializeField] [Tooltip("How quickly the player turns to face their movement direction.")]
    private float rotationSpeed = 10f;
    #endregion

    #region Variables: Jump & Gravity
    [Header("Jumping & Gravity")]
    [SerializeField] [Tooltip("How high the player jumps.")]
    private float jumpHeight = 2f;

    [SerializeField] [Tooltip("Strength of gravity pulling the player down.")]
    private float gravity = -19.62f;

    [SerializeField] [Tooltip("How long (in seconds) the player can still jump after leaving a ledge.")]
    private float coyoteTime = 0.2f;

    private float coyoteTimeCounter;
    private bool canDoubleJump;
    private Vector3 velocity; // Used to calculate falling/jumping speed
    #endregion

    #region Variables: Detection
    [Header("Detection")]
    [SerializeField] [Tooltip("The layer used for the ground.")]
    private LayerMask groundLayer;

    [SerializeField] [Tooltip("How far down to check for the ground.")]
    private float groundCheckDistance = 0.3f;

    private bool isGrounded;
    #endregion

    #region Private References
    private CharacterController controller; // The component that moves the player
    private Vector2 moveInput;              // Stores our WASD/Joystick data
    private bool isSprinting;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Automatically find the CharacterController attached to this object
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        ApplyMovement();
        ApplyGravity();
    }

    #region Input Methods
    // These methods are called by the "Player Input" component via "Send Messages"
    
    public void OnMove(InputValue value)
    {
        // Read the movement as a Vector2 (X and Y coordinates)
        moveInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        // Check if the sprint button is being held down
        isSprinting = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            // If we are within the "Coyote Time" window, we can jump
            if (coyoteTimeCounter > 0)
            {
                PerformJump();
                canDoubleJump = true; // Enable the ability to jump again in mid-air
            }
            // If we are in the air but have a double jump available
            else if (canDoubleJump)
            {
                PerformJump();
                canDoubleJump = false; // Consume the double jump
            }
        }
    }
    #endregion

    #region Movement Logic
    private void CheckGround()
    {
        // Use a Raycast to look straight down from the player's feet
        // It returns true if it hits anything on the "Ground" layer
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote timer while on ground
            if (velocity.y < 0)
            {