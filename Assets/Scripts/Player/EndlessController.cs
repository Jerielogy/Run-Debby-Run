using UnityEngine;

public class EndlessController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Colliders")]
    public CapsuleCollider2D standingCollider;
    public BoxCollider2D crouchingCollider;

    [Header("Movement Settings")]
    public float jumpForce = 12f;
    public bool isGrounded = true;
    public bool isCrouching = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (standingCollider != null) standingCollider.enabled = true;
        if (crouchingCollider != null) crouchingCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EndlessManager.Instance != null)
            {
                EndlessManager.Instance.TogglePause();
            }
        }
        if (isDead || Time.timeScale == 0f) return;

        // COUNTDOWN CONTROL: Freeze animation and lock input
        if (EndlessManager.Instance != null && EndlessManager.Instance.isCountingDown)
        {
            if (animator != null) animator.speed = 0; // Freeze animation frame
            return;
        }
        else
        {
            if (animator != null) animator.speed = 1; // Resume animation
        }

        // ANIMATOR SYNC: Update parameters (Requires 'yVelocity' Float in Animator)
        if (animator != null)
        {
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("yVelocity", rb.velocity.y);
        }

        HandleInput();
    }

    void HandleInput()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.JoystickButton0))
            && isGrounded && !isCrouching)
        {
            PerformJump();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded)
            StartCrouch();
        else if (Input.GetKeyUp(KeyCode.DownArrow))
            StopCrouch();
    }

    public void PerformJump()
    {
        if (isGrounded && !isCrouching)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            if (animator != null) animator.SetTrigger("Jump");

            // Trigger Jump SFX
            if (AudioManager.Instance != null) AudioManager.Instance.PlayJump();
        }
    }

    public void StartCrouch()
    {
        isCrouching = true;
        if (standingCollider != null) standingCollider.enabled = false;
        if (crouchingCollider != null) crouchingCollider.enabled = true;
        if (animator != null) animator.SetBool("IsCrouching", true);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCrouch();
    }

    public void StopCrouch()
    {
        isCrouching = false;
        if (crouchingCollider != null) crouchingCollider.enabled = false;
        if (standingCollider != null) standingCollider.enabled = true;
        if (animator != null) animator.SetBool("IsCrouching", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded && collision.relativeVelocity.y >= 0)
                isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (animator != null) animator.SetTrigger("Die");
        if (EndlessManager.Instance != null) EndlessManager.Instance.TriggerGameOver();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDeath();
    }
}