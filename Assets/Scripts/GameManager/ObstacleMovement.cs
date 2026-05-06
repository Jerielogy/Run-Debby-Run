using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    void Update()
    {
        if (EndlessManager.Instance != null && !EndlessManager.Instance.isGameOver)
        {
            // Move left based on the manager's global speed
            transform.Translate(Vector2.left * EndlessManager.Instance.worldSpeed * Time.deltaTime);
        }

        // Cleanup: Destroy if it moves too far off-screen
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}