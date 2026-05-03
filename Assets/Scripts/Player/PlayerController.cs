using UnityEngine;

public class PlayerController : MonoBehaviour
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

    // 0 = Keyboard, 1 = Voice, 2 = Joystick, 3 = Touch
    private int controlSchemeIndex = 0;

    [Header("Effects")]
    public ParticleSystem dust;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Load the saved control scheme from the menu
        controlSchemeIndex = PlayerPrefs.GetInt("ControlScheme", 0);

        // Initial collider state
        if (standingCollider != null) standingCollider.enabled = true;
        if (crouchingCollider != null) crouchingCollider.enabled = false;
    }

    void Update()
    {
        // Handle Pause first so it works even when Time.timeScale is 0
        HandlePauseInput();

        if (isDead || Time.timeScale == 0f) return;

        if (animator != null) animator.SetBool("IsGrounded", isGrounded);
        HandleInput();
    }

    void HandlePauseInput()
    {
        // Specifically check for Joystick Start Button (Button 7)
        if (controlSchemeIndex == 2 && Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0f;

            // 1. Tell the GameManager to show the Pause UI
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TogglePauseMenu(true);
            }

            Debug.Log("Game Paused and Panel Shown");
        }
        else
        {
            Time.timeScale = 1f;

            // 2. Tell the GameManager to hide the Pause UI
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TogglePauseMenu(false);
            }

            Debug.Log("Game Resumed and Panel Hidden");
        }
    }

    void HandleInput()
    {
        // Voice (1) and Touch (3) are handled by public functions[cite: 1]
        if (controlSchemeIndex == 1 || controlSchemeIndex == 3) return;

        // --- KEYBOARD (Scheme 0) ---
        if (controlSchemeIndex == 0)
        {
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
                && isGrounded && !isCrouching)
            {
                PerformJump();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow)) StartCrouch();
            else if (Input.GetKeyUp(KeyCode.DownArrow)) StopCrouch();
        }

        // --- JOYSTICK (Scheme 2) ---
        if (controlSchemeIndex == 2)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton0) && isGrounded && !isCrouching)
            {
                PerformJump();
            }

            float vAxis = Input.GetAxisRaw("VerticalJoystick");
            bool stickDown = (vAxis < -0.5f);
            bool buttonDown = Input.GetKey(KeyCode.JoystickButton2) || Input.GetKey(KeyCode.JoystickButton1);

            if (stickDown || buttonDown)
            {
                if (!isCrouching) StartCrouch();
            }
            else
            {
                if (isCrouching) StopCrouch();
            }
        }
    }

    // --- CORE MOVEMENT ---

    void PerformJump()
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

    void StartCrouch()
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

    // --- EXTERNAL GATED INTERFACE (Voice Control) ---

    public void Jump()
    {
        // Only allow voice commands if the Voice scheme is active[cite: 1]
        if (controlSchemeIndex == 1) PerformJump();
    }

    public void Crouch()
    {
        if (controlSchemeIndex == 1) StartCrouch();
    }

    public void ReleaseCrouch()
    {
        if (controlSchemeIndex == 1) StopCrouch();
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
    }

    public void TriggerDeathAnimation()
    {
        isDead = true;
        if (animator != null) animator.SetTrigger("Die");
        if (rb != null)
        {
            rb.drag = 0.5f;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(-5f, 5f), ForceMode2D.Impulse);
        }
        this.enabled = false;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDeath();
    }
}