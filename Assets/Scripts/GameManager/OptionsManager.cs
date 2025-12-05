using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Required for the TextMeshPro text

public class OptionsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Control Selector")]
    public TextMeshProUGUI controlDisplayText; // Assign your "Txt_CurrentControl" here

    // The list of control modes. 
    // 0 = Keyboard, 1 = Voice Control, 2 = Joystick (On-Screen)
    private string[] controlNames = { "Keyboard", "Voice Control", "Joystick" };
    private int currentIndex = 0;

    void Start()
    {
        // 1. Load Saved Music Volume (Default 1.0)
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicSlider.value = savedMusic;

        // 2. Load Saved SFX Volume (Default 1.0)
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSlider.value = savedSFX;

        // 3. Load Saved Control Scheme (Default 0 - Keyboard)
        currentIndex = PlayerPrefs.GetInt("ControlScheme", 0);

        // Safety check: ensure index is valid
        if (currentIndex < 0 || currentIndex >= controlNames.Length)
            currentIndex = 0;

        UpdateControlDisplay();
    }

    // --- AUDIO FUNCTIONS ---
    // Link this to Music Slider -> OnValueChanged
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);

        // Update the actual sound immediately if the DJ exists
        if (AudioManager.Instance != null)
            AudioManager.Instance.musicSource.volume = value;
    }

    // Link this to SFX Slider -> OnValueChanged
    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);

        if (AudioManager.Instance != null)
            AudioManager.Instance.sfxSource.volume = value;
    }

    // --- SELECTOR LOGIC ---

    // Link Left Button here with Parameter: -1
    // Link Right Button here with Parameter: 1
    public void CycleControlScheme(int direction)
    {
        currentIndex += direction;

        // Infinite Loop Logic (Wrap Around)
        if (currentIndex > controlNames.Length - 1)
            currentIndex = 0; // Wrap to Start
        else if (currentIndex < 0)
            currentIndex = controlNames.Length - 1; // Wrap to End

        // Save immediately
        PlayerPrefs.SetInt("ControlScheme", currentIndex);
        PlayerPrefs.Save();

        // Update the screen text
        UpdateControlDisplay();

        Debug.Log("Control set to: " + controlNames[currentIndex]);
    }

    private void UpdateControlDisplay()
    {
        if (controlDisplayText != null)
            controlDisplayText.text = controlNames[currentIndex];
    }

    // --- NAVIGATION ---
    public void GoBack()
    {
        // Check for transition manager to fade out nicely
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");
    }
}