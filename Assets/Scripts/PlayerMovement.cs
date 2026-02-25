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
    [Tooltip("The height of the player's collider when crouching