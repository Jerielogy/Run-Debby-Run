using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("BGM Clips")]
    public AudioClip backgroundMusic;

    [Header("SFX Clips")]
    public AudioClip jumpSound;
    public AudioClip crouchSound;
    public AudioClip dieSound;
    public AudioClip gameOverSound;
    public AudioClip scoreSound; // NEW: Added for scoring

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps music manager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusic();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // FIX: Instead of checking specific names, we check if the music is NOT playing.
        // This ensures music restarts in ANY level if it was previously stopped
        if (musicSource != null && !musicSource.isPlaying)
        {
            // Optional: Prevent music from playing in specific scenes (like "Intro" or "Credits")
            if (scene.name != "IntroScene")
            {
                PlayMusic();
                Debug.Log("BGM automatically resumed for: " + scene.name);
            }
        }
    }

    public void PlayMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            // Only start if it's not already playing to avoid "restarting" the song every scene
            if (!musicSource.isPlaying)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    // --- SFX FUNCTIONS ---

    public void PlayScore() // NEW: Function to play scoring sound
    {
        PlaySFX(scoreSound);
    }

    public void PlayJump() { PlaySFX(jumpSound); }
    public void PlayCrouch() { PlaySFX(crouchSound); }

    public void PlayDeath()
    {
        StopMusic();
        PlaySFX(dieSound);
    }

    public void PlayGameOver()
    {
        if (gameOverSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(gameOverSound);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}