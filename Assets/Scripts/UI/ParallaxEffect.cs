using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxFactor;
    // 0 = Moves WITH camera (Far background / Sky)
    // 1 = Moves WITH player (Foreground / Ground)
    // 0.5 = Moves half speed (Midground / Mountains)

    void Start()
    {
        startpos = transform.position.x;
        // Get the width of the sprite so we know when to loop it
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // 1. Calculate how far the camera has moved relative to the layer
        float temp = (cam.transform.position.x * (1 - parallaxFactor));
        float dist = (cam.transform.position.x * parallaxFactor);

        // 2. Move the background
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // 3. Infinite Looping Logic
        // If the camera moved past the end of the image, jump the image forward
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}