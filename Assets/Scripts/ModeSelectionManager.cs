using UnityEngine;

public class ModeSelectionManager : MonoBehaviour
{
    // Make sure these scene names match your Build Settings exactly
    public string storyModeScene = "Map_Experimental";
    public string endlessModeScene = "Endless_Experimental";
    public string mainMenuScene = "MainMenu";

    public void PlayStoryMode()
    {
        // Transitions to your original Philippine map gameplay
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(storyModeScene);
        }
    }

    public void PlayEndlessMode()
    {
        // Transitions to the high-score focused mode
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(endlessModeScene);
        }
    }

    public void BackToMainMenu()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(mainMenuScene);
        }
    }
}