using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Required for TMP_Dropdown

public class OptionsManager : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Dropdown controlDropdown; // Assign your new Dropdown here

    void Start()
    {
        // 1. Ensure the game isn't paused in the background
        Time.timeScale = 1f;

        // 2. Load the saved Control Scheme (Default 0: Keyboard)
        int savedScheme = PlayerPrefs.GetInt("ControlScheme", 0);

        // 3. Update the dropdown visual to match the save
        if (controlDropdown != null)
        {
            controlDropdown.value = savedScheme;
            controlDropdown.RefreshShownValue();

            // Link the function code-side so you don't forget in the Inspector
            controlDropdown.onValueChanged.AddListener(delegate {
                SaveControlScheme(controlDropdown.value);
            });
        }
    }

    // --- DROPDOWN LOGIC ---
    public void SaveControlScheme(int index)
    {
        // Save the index: 0 = Keyboard, 1 = Voice Control, 2 = Joystick
        PlayerPrefs.SetInt("ControlScheme", index);
        PlayerPrefs.Save();

        Debug.Log("Control Scheme updated to index: " + index);
    }

    // --- NAVIGATION ---
    public void GoBack()
    {
        // Return to the Main Menu
        SceneManager.LoadScene("MainMenu");
    }

    // --- RESET PROGRESS ---
    public void ResetGameProgress()
    {
        // Wipes all levels, map colors, and intro progress
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("Thesis Data Reset!");

        // Reload the scene to show the reset state
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}