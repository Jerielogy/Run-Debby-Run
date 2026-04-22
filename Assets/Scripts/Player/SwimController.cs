using UnityEngine;

public class SwimController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("3-Lane Settings")]
    [Tooltip("Order: 0 = Bottom, 1 = Middle, 2 = Top")]
    public float[] laneYPositions = { -1.23f, -0.4f, 0.7f };
    public float moveSpeed = 10f;
    public float forwardSpeed = 5f;
    private int currentLane = 1;

    [Header("State Settings")]
    public float waterGravity = 0f;
    public float normalGravity = 3f;
    public bool isSwimming = false;
    private bool isDead = false;

    private Vector3 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = normalGravity;
        targetPosition = new Vector3(transform.position.x, laneYPositions[currentLane], 0f);
    }

    void Update()
    {
        // FIX: Update animator BEFORE the pause check so he paddles on the "Ready" screen
        if (animator != null)
        {
            animator.SetBool("IsSwimming", isSwimming);
        }

        // Now stop execution if dead or game is paused
        if (isDead || Time.timeScale == 0f) return;

        if (isSwimming)
        {
            HandleLaneInput();
        }
    }

    void FixedUpdate()
    {
        if (isDead || Time.timeScale == 0) return;

        if (isSwimming)
        {
            // Smoothly slide to the target lane and move forward
            Vector3 nextPos = new Vector3(transform.position.x + forwardSpeed * Time.fixedDeltaTime, targetPosition.y, 0f);
            rb.MovePosition(Vector3.Lerp(transform.position, nextPos, Time.fixedDeltaTime * moveSpeed));
        }
    }

    void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            ChangeLane(1);

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            ChangeLane(-1);
    }

    void ChangeLane(int direction)
    {
        if (animator != null)
        {
            if (direction > 0) animator.SetTrigger("SwimUp");
            else animator.SetTrigger("Dive");
        }

        // 2. Then update the logic/position
        currentLane = Mathf.Clamp(currentLane + direction, 0, 2);
        targetPosition = new Vector3(transform.position.x, laneYPositions[currentLane], 0f);
    

}

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = normalGravity;

        if (animator != null) animator.SetTrigger("Die");
        // Add your AudioManager call here if needed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water")) EnterWater();
        if (collision.CompareTag("Collectible")) Destroy(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water")) ExitWater();
    }

    void EnterWater()
    {
        isSwimming = true;
        rb.gravityScale = waterGravity;
    }

    void ExitWater()
    {
        isSwimming = false;
        rb.gravityScale = normalGravity;
    }
}