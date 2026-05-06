using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    void Update()
    {
        if (EndlessManager.Instance == null || EndlessManager.Instance.isGameOver) return;

        // Use the EXACT same math as the background layer it sits on
        float speed = EndlessManager.Instance.worldSpeed * 0.9f;

        // Move LEFT
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < -15f) Destroy(gameObject);
    }
}