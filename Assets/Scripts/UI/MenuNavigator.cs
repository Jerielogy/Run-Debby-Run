using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    [Header("Scene Names")]
    public string optionsSceneName = "OptionsScene";
    public string menuSceneName = "MainMenu";

    // Link this to your Settings Button
    public void GoToOptions()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(optionsSceneName);
        }
        else
        {
            // Fallback if the transition manager is missing
            UnityEngine.SceneManagement.SceneManager.LoadScene(optionsSceneName);
        }
    }

    // Link this to the "Back" button inside the Options Menu
    public void GoToMenu()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(menuSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }
    }
}