using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // Required for scene transitions

public class EndlessManager : MonoBehaviour
{
    public static EndlessManager Instance;

    [Header("Game Stats")]
    public float worldSpeed = 0f;
    public float targetSpeed = 5f;
    public float score = 0f;
    private float highScore = 0f;
    public bool isGameOver = false;
    public bool isCountingDown = true;

    [Header("Speed & Difficulty")]
    public float accelerationRate = 0.1f; // Speed increase per second
    public float maxSpeed = 15f;          // The speed cap
    public float lastSpawnTime;           // Tracks the global spawn gap
    public float minGapBetweenSpawners = 1.5f;

    [Header("UI Text References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI countdownText;

    [Header("UI Panel References")]
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    private void Awake()
    {
        // Singleton pattern to allow other scripts to find this manager easily
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Reset game state
        Time.timeScale = 1f;
        worldSpeed = 0f;
        isCountingDown = true;
        score = 0;

        // Load record from local storage
        highScore = PlayerPrefs.GetFloat("HighScore", 0);
        UpdateHighScoreDisplay();

        // Ensure panels are hidden at start
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        StartCoroutine(StartCountdownRoutine());
    }

    void Update()
    {
        // 1. INPUT CHECK: Escape key to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // 2. CORE GAMEPLAY LOGIC (Runs only when not counting down/dead)
        if (!isGameOver && !isCountingDown && Time.timeScale != 0f)
        {
            // Increase speed over time (Acceleration)
            if (worldSpeed < maxSpeed)
            {
                worldSpeed += accelerationRate * Time.deltaTime;
                targetSpeed = worldSpeed;
            }

            // Update score based on distance (speed x time)
            score += worldSpeed * Time.deltaTime;
            if (scoreText != null)
                scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString("D5");

            // Real-time High Score Flip
            if (score > highScore)
            {
                highScore = score;
                UpdateHighScoreDisplay();
            }
        }

        // 3. THE GATE: Stop logic below this if game is paused or over
        if (isGameOver || Time.timeScale == 0f) return;
    }

    IEnumerator StartCountdownRoutine()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            for (int i = 3; i > 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }
            countdownText.text = "RUN!";
            yield return new WaitForSeconds(0.5f);
            countdownText.gameObject.SetActive(false);
        }

        isCountingDown = false;
        worldSpeed = targetSpeed;
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        worldSpeed = 0f;

        // Save high score to disk
        PlayerPrefs.SetFloat("HighScore", highScore);
        PlayerPrefs.Save();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOver();

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

    }

    // --- UI BUTTON FUNCTIONS ---

    public void TogglePause()
    {
        if (isCountingDown || isGameOver) return;

        bool isPaused = !pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneTransitionManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneTransitionManager.Instance.LoadScene("MainMenu"); // Ensure this matches your Build Settings
    }

    void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "Highscore: " + Mathf.FloorToInt(highScore).ToString("D5");
        }
    }
}