using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // --- 1. SINGLETON (Critical for other scripts to find this) ---
    public static GameManager Instance;

    [Header("Level Reward")]
    public PhotoData photoReward;

    [Header("Map Progression")]
    public int regionToUnlockIndex = 0;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public GameObject pauseMenuPanel;
    public GameObject pauseButton;

    [Header("UI Text")]
    public TextMeshProUGUI levelPopUpText;
    public TextMeshProUGUI scoreText;

    [Header("Game Settings")]
    public int score = 0;
    public int scoreToWin = 15;
    public float worldSpeed = 3f;
    public float speedMultiplier = 1.5f;

    private float initialWorldSpeed;
    private int currentLevel = 1;
    public bool isGameOver = false;

    [Header("External Components")]
    public VoiceController voiceController;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 0f; // FORCE GLOBAL FREEZE AT START

        isGameOver = false;
        score = 0;
        currentLevel = 1;
        initialWorldSpeed = worldSpeed;

        UpdateScoreText();

        if (levelPopUpText != null) levelPopUpText.gameObject.SetActive(false);
        StartCoroutine(ShowLevelPopUp("Level 1"));

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

    // --- NEW: GLOBAL UPDATE FOR INPUTS ---
    void Update()
    {
        // Handle Escape key to pause or resume across all levels
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGameOver) return;

            if (Time.timeScale > 0f)
            {
                PauseGame();
            }
            else
            {
                // Resume only if menu panel is inacive
                if (pauseMenuPanel != null && pauseMenuPanel.activeSelf)
                {
                    ResumeGame();
                }
            }
        }
    }

    // --- SCORE & LEVELING LOGIC ---
    public void AddScore(int pointsToAdd)
    {
        if (isGameOver || Time.timeScale == 0f) return;

        score += pointsToAdd;
        UpdateScoreText();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScore();
        }

        if (currentLevel == 1 && score >= 5)
        {
            currentLevel = 2;
            worldSpeed = initialWorldSpeed * speedMultiplier;
            StartCoroutine(ShowLevelPopUp("Level 2"));
        }
        else if (currentLevel == 2 && score >= 10)
        {
            currentLevel = 3;
            worldSpeed *= speedMultiplier;
            StartCoroutine(ShowLevelPopUp("Level 3"));
        }

        if (score >= scoreToWin)
        {
            UnlockRewards();
            WinLevel();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void UnlockRewards()
    {
        if (photoReward != null) photoReward.Unlock();

        if (regionToUnlockIndex >= 1 && regionToUnlockIndex <= 8)
        {
            PlayerPrefs.SetInt("LuzonProgress", regionToUnlockIndex);
            if (regionToUnlockIndex == 8) PlayerPrefs.SetInt("WorldProgress", 2);
        }
        else if (regionToUnlockIndex >= 9 && regionToUnlockIndex <= 12)
        {
            PlayerPrefs.SetInt("VisayasProgress", regionToUnlockIndex - 8);
            if (regionToUnlockIndex == 12) PlayerPrefs.SetInt("WorldProgress", 3);
        }
        else if (regionToUnlockIndex >= 13 && regionToUnlockIndex <= 18)
        {
            PlayerPrefs.SetInt("MindanaoProgress", regionToUnlockIndex - 12);
        }

        PlayerPrefs.Save();
    }

    // --- GAME STATES (Win, Lose, Pause) ---

    public void WinLevel()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);

        if (voiceController != null) voiceController.StopListening();
    }

    public void TriggerGameOverSequence()
    {
        if (isGameOver) return;
        isGameOver = true;

        PlayerController debby = FindObjectOfType<PlayerController>();
        if (debby != null) debby.TriggerDeathAnimation();
        else
        {
            SwimController alon = FindObjectOfType<SwimController>();
            if (alon != null) alon.TriggerDeathAnimation();
        }

        worldSpeed = 0;
        if (voiceController != null) voiceController.StopListening();
        StartCoroutine(ShowUIAfterDelay());
    }

    IEnumerator ShowUIAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);
        Time.timeScale = 0f;
    }

    public void PauseGame()
    {
        if (isGameOver) return;
        Time.timeScale = 0f; // Freeze game[cite: 1]

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);

        if (voiceController != null) voiceController.enabled = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Unfreeze game[cite: 1]

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);

        if (voiceController != null) voiceController.enabled = true;
    }

    // --- SCENE MANAGEMENT ---

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneTransitionManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneTransitionManager.Instance.LoadScene("MainMenu");
    }

    public void GoToMapSelection()
    {
        Time.timeScale = 1f;
        SceneTransitionManager.Instance.LoadScene("Map_Experimental");
    }

    private IEnumerator ShowLevelPopUp(string text)
    {
        if (levelPopUpText != null)
        {
            levelPopUpText.text = text;
            levelPopUpText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            levelPopUpText.gameObject.SetActive(false);
        }
    }
}