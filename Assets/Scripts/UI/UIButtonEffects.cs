using UnityEngine;
using UnityEngine.EventSystems; // Required for detecting hover/click

public class UIButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    public float scaleAmount = 1.1f; // Target size when hovering (1.1 = 110%)
    public float clickScale = 0.95f; // Target size when clicking (0.95 = 95%)
    public float animationSpeed = 10f; // How fast it resizes

    private Vector3 targetScale;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // Smoothly animate towards the target scale every frame
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    // --- MOUSE HOVER ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleAmount;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    // --- MOUSE CLICK ---
    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * clickScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // If we are still hovering over the button when we release, go back to hover size
        // Otherwise go back to normal
        targetScale = originalScale * scaleAmount;
    }
}