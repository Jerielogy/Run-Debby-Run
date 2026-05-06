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

    [Header("Effects")]
    public ParticleSystem dust;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Ensure Lakan starts in the correct state
        if (standingCollider != null) standingCollider.enabled = true;
        if (crouchingCollider != null) crouchingCollider.enabled = false;
    }

    void Update()
    {
        if (isDead || Time.timeScale == 0f) return;

        if (animator != null) animator.SetBool("IsGrounded", isGrounded);
        HandleInput();
    }

    void HandleInput()
    {
        // Keyboard Inputs
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded && !isCrouching)
        {
            PerformJump();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded)
        {
            StartCrouch();
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            StopCrouch();
        }

        // Joystick Button 0 for Jump and Axis for Crouch
        if (Input.GetKeyDown(KeyCode.JoystickButton0) && isGrounded && !isCrouching)
        {
            PerformJump();
        }
    }

    public void PerformJump()
    {
        if (isGrounded && !isCrouching)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            if (animator != null) animator.SetTrigger("Jump");
            if (dust != null) dust.Stop();
            if (AudioManager.Instance != null) AudioManager.Instance.PlayJump();
        }
    }

    public void StartCrouch()
    {
        if (isGrounded && !isCrouching)
        {
            isCrouching = true;
            if (standingCollider != null) standingCollider.enabled = false;
            if (crouchingCollider != null) crouchingCollider.enabled = true;
            if (animator != null) animator.SetBool("IsCrouching", true);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCrouch();
        }
    }

    public void StopCrouch()
    {
        if (isCrouching)
        {
            isCrouching = false;
            if (crouchingCollider != null) crouchingCollider.enabled = false;
            if (standingCollider != null) standingCollider.enabled = true;
            if (animator != null) animator.SetBool("IsCrouching", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded && collision.relativeVelocity.y >= 0)
            {
                isGrounded = true;
                if (dust != null) dust.Play();
            }
        }

        // Classic Dino Runner "Death" on Obstacle Hit
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null) animator.SetTrigger("Die");

        if (AudioManager.Instance != null) AudioManager.Instance.PlayDeath();

        // Notify the EndlessManager to stop the game and save highscore
        if (EndlessManager.Instance != null)
        {
            EndlessManager.Instance.TriggerGameOver();
        }
    }
}