using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("UI References")]
    [Tooltip("The parent panel containing the CanvasGroup component.")]
    public CanvasGroup fadePanel;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float minLoadingTime = 1.0f; // Adjust this for your 1.5s total target

    private bool isTransitioning = false;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initial State: Hidden and non-blocking
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
            fadePanel.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning) return; // Prevent double-triggering
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;

        // Wake up the panel (and the bird inside it)
        fadePanel.gameObject.SetActive(true);
        fadePanel.blocksRaycasts = true;

        // 1. FADE TO SOLID (Closing the curtain)
        // Uses unscaledDeltaTime so it works even if the game is paused!
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadePanel.alpha = timer / fadeDuration;
            yield return null;
        }
        fadePanel.alpha = 1f;

        Debug.Log("Scene loading started: " + sceneName);

        // 2. ASYNC LOAD
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float elapsed = 0f;
        // Wait for the minimum time OR until the scene is ready
        while (elapsed < minLoadingTime || op.progress < 0.9f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // 3. ACTIVATE NEW SCENE
        op.allowSceneActivation = true;
        while (!op.isDone)
        {
            yield return null;
        }

        // 4. FADE TO TRANSPARENT (Opening the curtain)
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadePanel.alpha = 1f - (timer / fadeDuration);
            yield return null;
        }

        // 5. THE CLEANUP
        FinishTransition();
    }

    private void FinishTransition()
    {
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;

        // This turns off the panel AND the bird child simultaneously
        fadePanel.gameObject.SetActive(false);

        isTransitioning = false;
        Debug.Log("Transition Finished and UI Disabled!");
    }
}