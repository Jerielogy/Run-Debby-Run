using UnityEngine;

public class EndlessBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Range(0, 1)]
    public float parallaxFactor;

    private SpriteRenderer sr;
    private Material mat;
    private Vector2 offset;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        // We create a unique material instance so layers don't scroll together
        mat = sr.material;
    }

    void Update()
    {
        if (EndlessManager.Instance == null || EndlessManager.Instance.isGameOver) return;

        // Instead of moving the object, we move the texture offset
        // This creates a perfect, infinite loop on a single object
        float moveSpeed = EndlessManager.Instance.worldSpeed * parallaxFactor * 0.1f;
        offset.x += moveSpeed * Time.deltaTime;

        // Apply the offset to the main texture
        mat.mainTextureOffset = offset;
    }

    // Cleanup to prevent material leaks
    void OnDestroy()
    {
        if (mat != null) Destroy(mat);
    }
}