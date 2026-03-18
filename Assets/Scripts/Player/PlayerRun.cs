using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : MonoBehaviour
{
    public float runSpeed = 8f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // --- THE FIX: ADD THIS CHECK ---
        // If the game is paused (Time.timeScale is 0) or the game is over, stop moving
        if (Time.timeScale == 0 || (GameManager.Instance != null && GameManager.Instance.isGameOver))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(runSpeed, rb.velocity.y);
    }
}