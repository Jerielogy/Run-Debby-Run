using UnityEngine;

public class EndlessBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Range(0, 1)]
    [Tooltip("0 = static, 1 = moves as fast as the player")]
    public float parallaxFactor;

    private float length;
    private float startPos;

    void Start()
    {
        startPos = transform.position.x;

        // Ensure the SpriteRenderer is attached to get the width
        if (GetComponent<SpriteRenderer>() != null)
        {
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }
        else
        {
            Debug.LogError("EndlessBackground needs a SpriteRenderer to loop correctly!");
        }
    }

    void Update()
    {
        // 1. Safety check for the Manager and Game Over state
        if (EndlessManager.Instance == null || EndlessManager.Instance.isGameOver)
            return;

        // 2. Calculate movement based on the Manager's global speed
        // This ensures the background speeds up as the game gets harder
        float distance = (EndlessManager.Instance.worldSpeed * parallaxFactor * Time.time);

        // 3. Apply movement
        transform.position = new Vector3(startPos - distance, transform.position.y, transform.position.z);

        // 4. Seamless Loop Logic
        // If the image moves past its own width, reset the starting position forward
        if (transform.position.x <= startPos - length)
        {
            startPos += length;
        }
    }
}