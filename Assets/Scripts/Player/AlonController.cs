using UnityEngine;

public class AlonController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement")]
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    private bool isGrounded;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        // 1. Grounded Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);

        // 2. Jump (Swim Stroke)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
            animator.SetTrigger("Jump");
        }

        // 3. Dive (Crouch)
        if (Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("IsCrouching", true);
        }
        else
        {
            animator.SetBool("IsCrouching", false);
        }
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.velocity = Vector2.zero; // Stop moving
    }
}