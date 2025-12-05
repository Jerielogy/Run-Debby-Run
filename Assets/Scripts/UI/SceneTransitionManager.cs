using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public CanvasGroup fadePanel; // Assign the Panel's CanvasGroup here
    public float fadeDuration = 0.5f;

    // Singleton pattern to access this easily from anywhere
    public static SceneTransitionManager Instance;

    private void Awake()
    {
        // Simple Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this canvas alive between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate if one already exists
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // 1. Fade OUT (Transparent to Black)
        fadePanel.blocksRaycasts = true; // Block clicks during transition
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = timer / fadeDuration;
            yield return null;
        }
        fadePanel.alpha = 1f;

        // 2. Load the Scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 3. Fade IN (Black to Transparent)
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = 1f - (timer / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false; // Allow clicks again
    }
}