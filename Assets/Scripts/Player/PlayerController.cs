using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;

    [Header("Movement Settings")]
    public float jumpForce = 12f;
    public bool isGrounded = true; // Public para makita ng ibang scripts

    [Header("Crouch Settings")]
    public Vector2 crouchSize = new Vector2(1f, 0.5f);
    private Vector2 standingSize;
    private Vector2 standingOffset;
    public bool isCrouching = false;
    private bool isDead = false;

    // 0 = Keyboard, 1 = Voice, 2 = Joystick, 3 = Touch
    // Mapapalitan yung value netong controlSchemeIndex depende sa options
    private int controlSchemeIndex = 0;
    
    [Header("Effects")] public ParticleSystem dust;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        if (boxCollider != null)
        {
            // Memorize standing shape
            standingSize = boxCollider.size;
            standingOffset = boxCollider.offset;
        }

        // Load the Control Setting basically PlayerPrefs save the options
        controlSchemeIndex = PlayerPrefs.GetInt("ControlScheme", 0);
    }

    void Update()
    {
        if (animator != null) animator.SetBool("IsGrounded", isGrounded);
        HandleInput();
    }

    void HandleInput()
    {

        if (isDead || Time.timeScale == 0f) return;    
        if (Time.timeScale == 0f) return;

        // --- 1. IGNORE EXTERNAL MODES ---
        // Kung Voice (1) or Touch (3) ang napili , do NOT check keys/buttons here.
        if (controlSchemeIndex == 1 || controlSchemeIndex == 3) return;


        // --- 2. KEYBOARD CONTROL (Scheme 0) ---
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


        // --- 3. JOYSTICK / GAMEPAD CONTROL (Scheme 2) ---
        if (controlSchemeIndex == 2)
        {
            // Jump (Button 0 is usually X on PS4, A on Xbox)
            if (Input.GetKeyDown(KeyCode.JoystickButton0) && isGrounded && !isCrouching)
            {
                PerformJump();
            }

            // Crouch (Vertical Axis or D-Pad)
            float vAxis = Input.GetAxisRaw("Vertical");

            if (vAxis < -0.5f) // Stick pushed down
            {
                if (!isCrouching) StartCrouch();
            }
            else // Stick released
            {
                if (isCrouching) StopCrouch();
            }
        }
    }

    // --- CORE MOVEMENT LOGIC ---

    void PerformJump()
    {
        if (isGrounded && !isCrouching)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;

            // Animation handles the Collider shrinking (Artist Method)
            if (animator != null) animator.SetTrigger("Jump");

            // Sound
            if (AudioManager.Instance != null) AudioManager.Instance.PlayJump();

            if (dust != null) dust.Stop();
        }
    }

    void StartCrouch()
    {
        if (isGrounded && !isCrouching && boxCollider != null)
        {
            isCrouching = true;

            // Smart Math: Keep feet on the ground
            float heightDifference = standingSize.y - crouchSize.y;
            float newOffsetY = standingOffset.y - (heightDifference / 2);

            boxCollider.size = crouchSize;
            boxCollider.offset = new Vector2(standingOffset.x, newOffsetY);

            if (animator != null) animator.SetBool("IsCrouching", true);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCrouch();
        }
    }

    public void StopCrouch()
    {
        if (isCrouching && boxCollider != null)
        {
            isCrouching = false;
            // Reset to standing shape
            boxCollider.size = standingSize;
            boxCollider.offset = standingOffset;

            if (animator != null) animator.SetBool("IsCrouching", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded)
            {
                isGrounded = true;

                // Safety reset just in case
                if (boxCollider != null) { boxCollider.size = standingSize; boxCollider.offset = standingOffset; }

                // Auto-stand logic:
                // Only stand up if the player is NOT holding the down button anymore
                bool holdingDown = false;
                if (controlSchemeIndex == 0) holdingDown = Input.GetKey(KeyCode.DownArrow);
                if (controlSchemeIndex == 2) holdingDown = Input.GetAxisRaw("Vertical") < -0.5f;
                // Voice/Touch don't "hold" down in the same way, so they auto-stand via their own StopCrouch calls.

                if (dust != null) dust.Play();

                if (isCrouching && !holdingDown)
                {
                    StopCrouch();
                }
            }
        }
    }

    // --- PUBLIC WRAPPERS (Used by Voice, Touch, and OnScreenControls) ---
    public void Jump()
    {
        // DOUBLE PROTECTION: 
        // If we are in Keyboard(0) or Joystick(2) mode, ignore external commands.
        if (controlSchemeIndex == 0 || controlSchemeIndex == 2) return;

        PerformJump();
    }

    public void Crouch()
    {
        // Note: For "Touch" controls, holding the button calls this repeatedly, 
        // but StartCrouch() checks "if (!isCrouching)" so it's safe.
        if (controlSchemeIndex == 0 || controlSchemeIndex == 2) return;

        StartCrouch();
    }

    public void TriggerDeathAnimation()
    {
        isDead = true;

        if (animator != null) animator.SetTrigger("Die");

        if (rb != null)
        {
            
            rb.drag = 0.5f;

            rb.AddForce(new Vector2(10f, 0), ForceMode2D.Impulse);
        }

        this.enabled = false;
    }
}