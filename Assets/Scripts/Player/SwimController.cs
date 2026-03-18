using UnityEngine;

public class SwimController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Swimming Settings")]
    public float forwardSpeed = 5f;   // horizontal speed
    public float riseForce = 8f;
    public float diveForce = 6f;
    public float waterGravity = 0.5f;
    public float waterDrag = 1f;
    public float normalGravity = 3f;

    public bool isSwimming = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = normalGravity;
    }

    void Update()
    {
        if (isDead || Time.timeScale == 0f) return;

        // Keep swimming animation active
        if (animator != null)
            animator.SetBool("IsSwimming", isSwimming);

        if (isSwimming)
            HandleSwimmingInput();
    }

    void FixedUpdate()
    {
        // --- THE FIX: ADD THIS CHECK ---
        if (isDead || Time.timeScale == 0) return;

        if (isSwimming)
        {
            rb.velocity = new Vector2(forwardSpeed, rb.velocity.y);
        }
    }

    void HandleSwimmingInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Rise();

        if (Input.GetKeyDown(KeyCode.DownArrow))
            Dive();
    }

    void Rise()
    {
        rb.velocity = new Vector2(rb.velocity.x, riseForce);
        if (animator != null)
            animator.SetTrigger("SwimUp");
    }

    void Dive()
    {
        rb.velocity = new Vector2(rb.velocity.x, -diveForce);
        if (animator != null)
            animator.SetTrigger("Dive");
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Stop ALL movement
        rb.velocity = Vector2.zero;
        rb.gravityScale = normalGravity;
        rb.drag = 0f;

        // Play death animation
        if (animator != null)
            animator.SetTrigger("Die");
    }

    // Only triggers Die when hitting obstacles or enemies
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Obstacle")))
            Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
            EnterWater();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
            ExitWater();
    }

    void EnterWater()
    {
        isSwimming = true;
        rb.gravityScale = waterGravity;
        rb.drag = waterDrag;

        // DO NOT zero velocity; forward movement happens in FixedUpdate
    }

    void ExitWater()
    {
        isSwimming = false;
        rb.gravityScale = normalGravity;
        rb.drag = 0f;
    }
}