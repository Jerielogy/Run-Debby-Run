using UnityEngine;
using System.Collections;

public class GuideHandAnimation : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 10f; // How far it moves up/down
    public float floatFrequency = 2f;  // How fast it moves

    [Header("Clicking Settings")]
    public float clickInterval = 2f;   // Time between clicks
    public float clickScale = 0.8f;    // How much it shrinks when "clicking"

    private Vector3 startPos;
    private Vector3 startScale;

    void Start()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
        StartCoroutine(ClickRoutine());
    }

    void Update()
    {
        // Makes the hand float up and down slightly
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }

    IEnumerator ClickRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(clickInterval);

            // Simulate the "Click" down
            float timer = 0;
            while (timer < 0.15f)
            {
                timer += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, startScale * clickScale, timer / 0.15f);
                yield return null;
            }

            // Simulate the "Release" up
            timer = 0;
            while (timer < 0.15f)
            {
                timer += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale * clickScale, startScale, timer / 0.15f);
                yield return null;
            }
        }
    }
}