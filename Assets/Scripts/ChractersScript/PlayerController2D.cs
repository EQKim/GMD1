using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private Animator animator;   // Animator on Visual
    [SerializeField] private Transform visual;    // Visual transform (for flipping)

    private Rigidbody2D rb;
    private bool grounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Optional auto-wire if you forgot to drag refs
        if (visual == null)
        {
            var t = transform.Find("Visual");
            if (t != null) visual = t;
        }

        if (animator == null && visual != null)
            animator = visual.GetComponent<Animator>();
    }

    private void Update()
    {
        // Horizontal input (A/D or Left/Right)
        float moveX = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
        }

        // Ground check
        grounded = false;
        if (groundCheck != null)
            grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump (Space)
        bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        if (jumpPressed && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Apply horizontal movement (keep current Y velocity)
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        // Flip sprite
        if (visual != null && moveX != 0f)
        {
            Vector3 s = visual.localScale;
            s.x = Mathf.Abs(s.x) * (moveX > 0f ? 1f : -1f);
            visual.localScale = s;
        }

        // Animator params
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveX));
            animator.SetBool("Grounded", grounded);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}