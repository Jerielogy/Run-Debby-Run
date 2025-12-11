using UnityEngine;

public class GroundRepeater : MonoBehaviour
{
    private float groundWidth;

    void Start()
    {
        // 1. Get the sprite's width (Width of the image * Scale of the object)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        groundWidth = sr.size.x * transform.localScale.x;
    }

    void Update()
    {
        // 2. Get Camera position
        float camX = Camera.main.transform.position.x;
        float myX = transform.position.x;

        // 3. If the camera has passed this tile completely...
        // (We add a buffer so it doesn't pop visible on screen)
        if (camX > myX + groundWidth)
        {
            Reposition();
        }
    }

    void Reposition()
    {
        // 4. Move forward exactly 2 widths (leapfrog the other tile)
        Vector2 jumpOffset = new Vector2(groundWidth * 2f, 0);
        transform.position = (Vector2)transform.position + jumpOffset;
    }
}