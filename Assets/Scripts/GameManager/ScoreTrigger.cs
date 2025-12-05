using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    public int pointsToGive = 1; // We can change this in the Inspector
    private GameManager gameManager;
    private bool hasTriggered = false; // Prevents double-scoring

    void Start()
    {
        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
    }

    // This function runs when a trigger collider is entered
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered is tagged "Player"
        // AND that we haven't already scored this obstacle
        if (other.gameObject.CompareTag("Player") && !hasTriggered)
        {
            // We scored!
            hasTriggered = true;

            // Tell the GameManager to add our points
            gameManager.AddScore(pointsToGive);

            // Optional: Destroy this trigger so it can't be hit again
            // Destroy(gameObject); 
        }
    }
}