using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private bool isMainCharacter = true;

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

    [Header("Flip Character")]
    [SerializeField] private bool startFacingRight = true;

    private Rigidbody2D rb;
    private bool grounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (visual == null)
        {
            var t = transform.Find("Visual");
            if (t != null) visual = t;
        }

        if (animator == null && visual != null)
            animator = visual.GetComponent<Animator>();
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
        // 🚫 If not main character, do nothing
        if (!isMainCharacter)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
            return;
        }

        float moveX = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
        }

        grounded = false;
        if (groundCheck != null)
            grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        if (jumpPressed && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

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

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}