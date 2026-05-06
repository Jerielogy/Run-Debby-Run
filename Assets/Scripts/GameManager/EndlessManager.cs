using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndlessManager : MonoBehaviour
{
    public static EndlessManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
    public GameObject gameOverPanel;

    [Header("Game Settings")]
    public float worldSpeed = 5f;
    public float speedIncrement = 0.1f;
    public float maxSpeed = 15f;

    private float score = 0f;
    private int highscore = 0;
    public bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        highscore = PlayerPrefs.GetInt("EndlessHighscore", 0);
        UpdateHighscoreUI();
    }

    void Update()
    {
        if (isGameOver) return;

        if (worldSpeed < maxSpeed)
        {
            worldSpeed += speedIncrement * Time.deltaTime;
        }

        score += worldSpeed * Time.deltaTime;

        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();

        if (score > highscore)
        {
            highscore = Mathf.FloorToInt(score);
            UpdateHighscoreUI();
        }
    }

    void UpdateHighscoreUI()
    {
        if (highscoreText != null)
            highscoreText.text = "Highscore: " + highscore.ToString("D5");
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

        PlayerPrefs.SetInt("EndlessHighscore", highscore);
        PlayerPrefs.Save();

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}