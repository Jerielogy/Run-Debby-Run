using UnityEngine;
using TMPro;

public class DisplayModeController : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Dropdown displayModeDropdown;

    // Unique key to ensure no conflicts with other save data
    private const string DisplayPrefKey = "Debby_Display_Mode_Index";

    void Start()
    {
        // 1. Load the saved preference (Defaults to 0/Fullscreen if first time playing)
        int savedMode = PlayerPrefs.GetInt(DisplayPrefKey, 0);

        Debug.Log("DisplayModeController: Loading saved state " + savedMode);

        if (displayModeDropdown != null)
        {
            // 2. IMPORTANT: Use SetValueWithoutNotify to update the text label
            // without triggering the RefreshDisplayMode function again.
            displayModeDropdown.SetValueWithoutNotify(savedMode);
        }
        else
        {
            Debug.LogError("DisplayModeController: No Dropdown assigned in the Inspector!");
        }

        // 3. Apply the setting immediately so the window/fullscreen state is correct
        ApplyDisplayMode(savedMode);
    }

    // This is the function you link to the "Dynamic int" section of the Dropdown
    public void RefreshDisplayMode(int val)
    {
        Debug.Log("DisplayModeController: User selected index " + val);

        // Save the choice immediately
        PlayerPrefs.SetInt(DisplayPrefKey, val);
        PlayerPrefs.Save();

        // Apply the visual change
        ApplyDisplayMode(val);
    }

    private void ApplyDisplayMode(int index)
    {
        if (index == 0)
        {
            // INDEX 0: BORDERLESS FULLSCREEN
            // Uses native resolution for the best PC experience
            Resolution res = Screen.currentResolution;
            Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
            Debug.Log("DisplayModeController: Applied Fullscreen");
        }
        else
        {
            // INDEX 1: WINDOWED MODE
            // Forces a 720p window so it is clearly smaller than a 1080p monitor
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            Debug.Log("DisplayModeController: Applied 720p Windowed");
        }
    }
}