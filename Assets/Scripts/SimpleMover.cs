using UnityEngine;

public class RandomSimpleMover : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform pointA;
    public Transform pointB;

    [Header("Randomization")]
    public float minSpeed = 2.0f;
    public float maxSpeed = 5.0f;
    public float minWaitTime = 1.0f;
    public float maxWaitTime = 10.0f;

    private float currentSpeed;
    private float waitTimer;
    private bool isWaiting = false;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        ResetToStart();
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                // TIME IS UP: Teleport back to start and fly again!
                transform.position = pointA.position;
                currentSpeed = Random.Range(minSpeed, maxSpeed);
                isWaiting = false;
                if (sprite != null) sprite.enabled = true;
            }
            return;
        }

        // Move towards Point B
        transform.position = Vector3.MoveTowards(transform.position, pointB.position, currentSpeed * Time.deltaTime);

        // Check if reached Point B
        if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
        {
            StartWaiting();
        }
    }

    void StartWaiting()
    {
        isWaiting = true;
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
        if (sprite != null) sprite.enabled = false;
    }

    void ResetToStart()
    {
        transform.position = pointA.position;
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        isWaiting = true; // Start with a random delay so they don't all launch at 0:00
        waitTimer = Random.Range(0f, maxWaitTime);
    }
}