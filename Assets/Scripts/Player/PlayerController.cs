using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Colliders")]
    public CapsuleCollider2D standingCollider; // Drag your Capsule here
    public BoxCollider2D crouchingCollider;    // Drag your new Box here

    [Header("Movement Settings")]
    public float jumpForce = 12f;
    public bool isGrounded = true;
    public bool isCrouching = false;
    private bool isDead = false;

    // 0 = Keyboard, 1 = Voice, 2 = Joystick, 3 = Touch
    private int controlSchemeIndex = 0;

    [Header("Effects")] public ParticleSystem dust;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controlSchemeIndex = PlayerPrefs.GetInt("ControlScheme", 0);

        // Ensure we start in the correct state (Standing ON, Crouching OFF)
        if (standingCollider != null) standingCollider.enabled = true;
        if (crouchingCollider != null) crouchingCollider.enabled = false;
    }

    void Update()
    {
        if (animator != null) animator.SetBool("IsGrounded", isGrounded);
        HandleInput();
    }

    void HandleInput()
    {
        if (isDead || Time.timeScale == 0f) return;

        // Voice (1) and Touch (3) are handled by public functions below
        if (controlSchemeIndex == 1 || controlSchemeIndex == 3) return;

        // --- KEYBOARD (Scheme 0) ---
        if (controlSchemeIndex == 0)
        {
            // Jump
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
                && isGrounded && !isCrouching)
            {
                PerformJump();
            }

            // Crouch
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCrouch();
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                StopCrouch();
            }
        }

        // --- JOYSTICK (Scheme 2) ---
        if (controlSchemeIndex == 2)
        {
            // JUMP: Usually Button 0 (A on Xbox, X on PS, Bottom Button on Generic)
            if (Input.GetKeyDown(KeyCode.JoystickButton0) && isGrounded && !isCrouching)
            {
                PerformJump();
            }

            // CROUCH LOGIC (Stick OR Buttons)

            // 1. Check Analog Stick (Requires "VerticalJoystick" setup in Input Manager)
            float vAxis = Input.GetAxisRaw("VerticalJoystick");
            bool stickDown = (vAxis < -0.5f);

            // 2. Check Face Buttons (Usually Circle/B is Button 1 or 2)
            // Adjust these numbers if your specific controller uses different IDs
            bool buttonDown = Input.GetKey(KeyCode.JoystickButton2) || Input.GetKey(KeyCode.JoystickButton1);

            // 3. Trigger Crouch
            if (stickDown || buttonDown)
            {
                if (!isCrouching) StartCrouch();
            }
            else
            {
                // Only stand up if BOTH stick and buttons are released
                if (isCrouching) StopCrouch();
            }
        }
    }

    // --- CORE LOGIC ---

    void PerformJump()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayJump();

        if (isGrounded && !isCrouching)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            if (animator != null) animator.SetTrigger("Jump");
            if (dust != null) dust.Stop();
        }
    }

    void StartCrouch()
    {
        if (isGrounded && !isCrouching)
        {
            isCrouching = true;

            // --- THE COLLIDER SWAP ---
            if (standingCollider != null) standingCollider.enabled = false;
            if (crouchingCollider != null) crouchingCollider.enabled = true;
            // ------------------------

            if (animator != null) animator.SetBool("IsCrouching", true);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayCrouch();
    }

    public void StopCrouch()
    {
        if (isCrouching)
        {
            isCrouching = false;

            // --- SWAP BACK ---
            if (crouchingCollider != null) crouchingCollider.enabled = false;
            if (standingCollider != null) standingCollider.enabled = true;
            // -----------------

            if (animator != null) animator.SetBool("IsCrouching", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Only land if falling
            if (!isGrounded && collision.relativeVelocity.y >= 0)
            {
                isGrounded = true;
                if (dust != null) dust.Play();

                // Auto-stand logic prevents getting stuck in crouch upon landing
                bool holdingDown = false;

                if (controlSchemeIndex == 0) holdingDown = Input.GetKey(KeyCode.DownArrow);
                if (controlSchemeIndex == 2)
                {
                    float vAxis = Input.GetAxisRaw("VerticalJoystick");
                    bool btn = Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.JoystickButton2);
                    holdingDown = (vAxis < -0.5f) || btn;
                }

                if (isCrouching && !holdingDown) StopCrouch();
            }
        }
    }


    public void Jump()
    {
        if (controlSchemeIndex != 0) PerformJump();
    }

    public void Crouch()
    {
        if (controlSchemeIndex != 0) StartCrouch();
    }

    
    public void ReleaseCrouch()
    {
        if (controlSchemeIndex != 0) StopCrouch();
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