using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float rotationSpeed = 700f;

    [Header("Jumping & Gravity")]
    [SerializeField] private float jumpHeight = 2.5f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float coyoteTime = 0.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool isSprinting;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private bool canDoubleJump;

    private Transform cameraTransform;
    
    // NEW: We need a brain connection!
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // NEW: This looks inside Player1 and finds the Animator on your 3D model
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        CheckGround();
        MovePlayer();
        ApplyGravity();
        UpdateAnimations(); // NEW: Run the animation math every frame
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
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * moveInput.y) + (right * moveInput.x);
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

    // NEW: The Animation Logic!
    private void UpdateAnimations()
    {
        // If we don't have an animator, skip this
        if (animator == null) return;

        // Calculate how fast the player is moving horizontally (ignoring falling speed)
        float currentSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        // Send those numbers to the Animator parameters you made!
        animator.SetFloat("Speed", currentSpeed);
      animator.SetBool("isGrounded", isGrounded);
}
#endregion
}