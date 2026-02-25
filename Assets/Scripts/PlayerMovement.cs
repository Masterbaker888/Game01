using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Movement Parameters
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
    
    [SerializeField] [Tooltip("Buffer time (seconds) you can still jump after falling off a ledge.")]
    private float coyoteTime = 0.2f;
    #endregion

    #region Detection Settings
    [Header("Ground Detection")]
    [SerializeField] [Tooltip("Set this to the 'Ground' layer in the inspector.")]
    private LayerMask groundLayer;
    
    [SerializeField] [Tooltip("How far down from the center to look for the floor.")]
    private float groundCheckDistance = 1.1f; 
    #endregion

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool isSprinting;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private bool canDoubleJump;

    void Start()
    {
        controller = GetComponent<CharacterController>();
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
        // X-RAY VISION: This will print to the console when you press/release Sprint
        Debug.Log("Sprint button is pressed: " + isSprinting); 
    }

    public void OnJump(InputValue value)
    {
        // X-RAY VISION: This will print every time you press the jump button
        Debug.Log("Jump button was pressed! Am I grounded? " + isGrounded); 

        if (value.isPressed)
        {
            if (coyoteTimeCounter > 0)
            {
                PerformJump();
                canDoubleJump = true;
                Debug.Log("Performed Normal Jump!");
            }
            else if (canDoubleJump)
            {
                PerformJump();
                canDoubleJump = false;
                Debug.Log("Performed Double Jump!");
            }
        }
    }
    #endregion

    #region Core Logic
    private void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // X-RAY VISION: Draws a red line in your Scene view so you can see the ground check
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);

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
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

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