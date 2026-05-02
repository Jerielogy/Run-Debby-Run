using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Dropdown controlDropdown;

    [Header("Audio Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Reset Confirmation")] // NEW: Confirmation Panel Reference
    public GameObject confirmationPanel;

    void Start()
    {
        Time.timeScale = 1f;

        // 1. Load Control Scheme
        int savedScheme = PlayerPrefs.GetInt("ControlScheme", 0);
        if (controlDropdown != null)
        {
            controlDropdown.value = savedScheme;
            controlDropdown.RefreshShownValue();
            controlDropdown.onValueChanged.AddListener(delegate {
                SaveControlScheme(controlDropdown.value);
            });
        }

        // 2. Load Audio Levels (Default to 1.0/Full Volume)
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (musicSlider != null) musicSlider.value = savedMusic;

        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        // Ensure the panel is hidden on start
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
    }

    // --- AUDIO LOGIC ---
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.volume = value;
        }
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        if (AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
        {
            AudioManager.Instance.sfxSource.volume = value;
        }
    }

    // --- DROPDOWN LOGIC ---
    public void SaveControlScheme(int index)
    {
        PlayerPrefs.SetInt("ControlScheme", index);
        PlayerPrefs.Save();
    }

    // --- NAVIGATION ---
    public void GoBack() { SceneTransitionManager.Instance.LoadScene("MainMenu"); }

    // RESET PROGRESS LOGIC

    public void OpenResetConfirmation()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
    }

    public void ConfirmReset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        SceneTransitionManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CloseResetConfirmation()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }
}