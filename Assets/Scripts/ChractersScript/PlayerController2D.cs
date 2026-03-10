using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private bool isControllable = true;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visual;
    [SerializeField] private PlayerAttackHitbox attackHitbox;

    [Header("Flip Character")]
    [SerializeField] private bool startFacingRight = true;

    [Header("Keyboard Input Keys")]
    [SerializeField] private bool useKeyboard = true;
    [SerializeField] private Key moveLeftKey = Key.A;
    [SerializeField] private Key moveRightKey = Key.D;
    [SerializeField] private Key jumpKey = Key.Q;
    [SerializeField] private Key attackKey = Key.E;

    [Header("Gamepad Input")]
    [SerializeField] private bool useGamepad = true;
    [SerializeField] private int gamepadIndex = 0;

    [Tooltip("Xbox B / east button by default")]
    [SerializeField] private GamepadButton jumpButton = GamepadButton.East;

    [Tooltip("Xbox X / west button by default")]
    [SerializeField] private GamepadButton attackButton = GamepadButton.West;

    [SerializeField] private float stickDeadzone = 0.2f;

    [Header("Attack")]
    [SerializeField] private float heavyAttackHoldTime = 0.35f;
    [SerializeField] private string quickAttackTriggerName = "QuickAttack";
    [SerializeField] private string heavyAttackTriggerName = "HeavyAttack";

    private Rigidbody2D rb;
    private bool grounded;

    private bool attackHeld;
    private float attackHeldTime;
    private float attackStartTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (visual == null)
        {
            Transform t = transform.Find("Visual");
            if (t != null)
                visual = t;
        }

        if (animator == null && visual != null)
            animator = visual.GetComponent<Animator>();

        if (attackHitbox == null)
            attackHitbox = GetComponentInChildren<PlayerAttackHitbox>();
    }

    private void Start()
    {
        if (visual != null)
        {
            Vector3 s = visual.localScale;
            s.x = Mathf.Abs(s.x) * (startFacingRight ? 1f : -1f);
            visual.localScale = s;
        }
    }

    private void Update()
    {
        if (!isControllable)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetBool("Grounded", grounded);
            }

            ResetAttackInput();
            return;
        }

        grounded = false;
        if (groundCheck != null)
        {
            grounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        float moveX = ReadMoveInput();

        HandleJump();
        HandleAttack();

        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        if (visual != null && moveX != 0f)
        {
            Vector3 s = visual.localScale;
            s.x = Mathf.Abs(s.x) * (moveX > 0f ? 1f : -1f);
            visual.localScale = s;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveX));
            animator.SetBool("Grounded", grounded);
        }
    }

    private Gamepad GetAssignedGamepad()
    {
        if (!useGamepad)
            return null;

        if (Gamepad.all.Count <= gamepadIndex)
            return null;

        return Gamepad.all[gamepadIndex];
    }

    private float ReadMoveInput()
    {
        float moveX = 0f;

        if (useKeyboard && Keyboard.current != null)
        {
            if (Keyboard.current[moveLeftKey].isPressed)
                moveX -= 1f;

            if (Keyboard.current[moveRightKey].isPressed)
                moveX += 1f;
        }

        Gamepad pad = GetAssignedGamepad();
        if (pad != null)
        {
            float stickX = pad.leftStick.ReadValue().x;

            if (Mathf.Abs(stickX) > stickDeadzone)
                moveX = stickX;

            if (pad.dpad.left.isPressed)
                moveX = -1f;
            else if (pad.dpad.right.isPressed)
                moveX = 1f;
        }

        return Mathf.Clamp(moveX, -1f, 1f);
    }

    private void HandleJump()
    {
        bool jumpPressed = false;

        if (useKeyboard && Keyboard.current != null && Keyboard.current[jumpKey].wasPressedThisFrame)
            jumpPressed = true;

        Gamepad pad = GetAssignedGamepad();
        if (pad != null && pad[jumpButton].wasPressedThisFrame)
            jumpPressed = true;

        if (jumpPressed && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleAttack()
    {
        bool pressedThisFrame = false;
        bool releasedThisFrame = false;
        bool isPressed = false;

        if (useKeyboard && Keyboard.current != null)
        {
            KeyControl attackControl = Keyboard.current[attackKey];
            if (attackControl != null)
            {
                if (attackControl.wasPressedThisFrame) pressedThisFrame = true;
                if (attackControl.wasReleasedThisFrame) releasedThisFrame = true;
                if (attackControl.isPressed) isPressed = true;
            }
        }

        Gamepad pad = GetAssignedGamepad();
        if (pad != null)
        {
            ButtonControl button = pad[attackButton];
            if (button != null)
            {
                if (button.wasPressedThisFrame) pressedThisFrame = true;
                if (button.wasReleasedThisFrame) releasedThisFrame = true;
                if (button.isPressed) isPressed = true;
            }
        }

        if (pressedThisFrame)
        {
            attackHeld = true;
            attackStartTime = Time.time;
            attackHeldTime = 0f;
        }

        if (attackHeld && isPressed)
        {
            attackHeldTime = Time.time - attackStartTime;
        }

        if (attackHeld && releasedThisFrame)
        {
            attackHeld = false;

            if (animator != null)
            {
                if (attackHeldTime >= heavyAttackHoldTime)
                    animator.SetTrigger(heavyAttackTriggerName);
                else
                    animator.SetTrigger(quickAttackTriggerName);
            }

            attackHeldTime = 0f;
        }
    }

    private void ResetAttackInput()
    {
        attackHeld = false;
        attackHeldTime = 0f;
        attackStartTime = 0f;
    }

    public void AE_EnableQuickAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.EnableQuickAttack();
    }

    public void AE_EnableHeavyAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.EnableHeavyAttack();
    }

    public void AE_DisableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.DisableAttack();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}