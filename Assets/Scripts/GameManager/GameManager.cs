using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // --- 1. SINGLETON (Critical for other scripts to find this) ---
    public static GameManager Instance;

    [Header("Level Reward")]
    public PhotoData photoReward; // The photo needed for the gallery

    [Header("Map Progression")]
    [Tooltip("Which Region ID does this level unlock? (Set 1 to unlock CAR, 2 for Region 2, etc.)")]
    public int regionToUnlockIndex = 0;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel; // Win Screen
    public GameObject pauseMenuPanel;     // Pause Screen
    public GameObject pauseButton;        // Pause Button (||)

    [Header("UI Text")]
    public TextMeshProUGUI levelPopUpText;
    public TextMeshProUGUI scoreText;

    [Header("Game Settings")]
    public int score = 0;
    public int scoreToWin = 40;
    public float worldSpeed = 3f;
    public float speedMultiplier = 1.5f;

    private float initialWorldSpeed;
    private int currentLevel = 1;
    public bool isGameOver = false;

    [Header("External Components")]
    public VoiceController voiceController;

    void Awake()
    {
        // Setup Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f; // Ensure game is running
        isGameOver = false;
        score = 0;
        currentLevel = 1;
        initialWorldSpeed = worldSpeed;

        UpdateScoreText();

        // UI Setup
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);

        // Show "Level 1"
        if (levelPopUpText != null) StartCoroutine(ShowLevelPopUp("Level 1"));
    }

    // --- SCORE & LEVELING LOGIC ---
    public void AddScore(int pointsToAdd)
    {
        if (isGameOver || Time.timeScale == 0f) return;

        score += pointsToAdd;
        UpdateScoreText();

        // Level 2 Transition (Score 15)
        if (currentLevel == 1 && score >= 15)
        {
            currentLevel = 2;
            worldSpeed = initialWorldSpeed * speedMultiplier;
            StartCoroutine(ShowLevelPopUp("Level 2"));
            Debug.Log("Level 2 reached! Speed Up.");
        }
        // Level 3 Transition (Score 25)
        else if (currentLevel == 2 && score >= 25)
        {
            currentLevel = 3;
            worldSpeed *= speedMultiplier;
            StartCoroutine(ShowLevelPopUp("Level 3"));
            Debug.Log("Level 3 reached! Speed Up.");
        }

        // Win Condition
        if (score >= scoreToWin)
        {
            UnlockRewards();
            WinLevel();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    // --- REWARD & PROGRESSION ---
    void UnlockRewards()
    {
        // 1. Unlock Photo
        if (photoReward != null)
        {
            photoReward.Unlock();
            Debug.Log("WINNER! Unlocked Photo: " + photoReward.name);
        }

        // 2. Unlock Next Map Region
        if (regionToUnlockIndex > 0)
        {
            PlayerPrefs.SetInt("RegionUnlocked_" + regionToUnlockIndex, 1);
            PlayerPrefs.Save();
            Debug.Log("Map Region " + regionToUnlockIndex + " Unlocked!");
        }
    }

    // --- GAME STATES (Win, Lose, Pause) ---

    public void WinLevel()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f; // Freeze

        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);

        if (voiceController != null) voiceController.StopListening();
    }

    // Called by PlayerCollision
    public void TriggerGameOverSequence()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Death sequence started . . . ");

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) player.TriggerDeathAnimation();

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

        Time.timeScale = 0f; // Freeze

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);

        // Disable voice while paused to prevent accidental jumps
        if (voiceController != null) voiceController.enabled = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Unfreeze

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);

        if (voiceController != null) voiceController.enabled = true;
    }

    // --- SCENE MANAGEMENT ---

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Unfreeze before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToMapSelection()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapSelection");
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