using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // For Background Music (Looping)
    public AudioSource sfxSource;   // For Sound Effects (Jump, Die, etc.)

    [Header("Sound Clips")]
    public AudioClip backgroundMusic;
    public AudioClip jumpSound;
    public AudioClip crouchSound;
    public AudioClip dieSound;
    public AudioClip gameOverSound;

    
    void Awake()
    {
        // Singleton Pattern: Ensure only one Audio Manager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep music playing when switching scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates if we reload the scene
        }
    }

    void Start()
    {
        PlayMusic();
    }

    // --- MUSIC FUNCTIONS ---
    void OnEnable()
    {
        // When the AudioManager is enabled, subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // When the AudioManager is disabled/destroyed, unsubscribe to prevent errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only restart music if we are NOT loading into the Main Menu/Map Selection
        // Assuming your Main Menu is the first scene (Index 0 or named "MainMenu")
        // Check your Build Settings for the exact name/index.

        if (scene.name == "Level1" || scene.name == "Level2" || scene.name == "MapSelection")
        {
            // The music might have been stopped by PlayDeath() or PlayGameOver()
            if (musicSource != null && !musicSource.isPlaying)
            {
                PlayMusic();
                Debug.Log("BGM restarted for new scene.");
            }
        }
    }
    
    public void PlayMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    // --- SFX FUNCTIONS ---
    public void PlayJump()
    {
        PlaySFX(jumpSound);
    }

    public void PlayCrouch()
    {
        PlaySFX(crouchSound); // Optional: Swishing sound
    }

    public void PlayDeath()
    {
        StopMusic(); // Stop the happy music!
        PlaySFX(dieSound);
    }

    public void PlayGameOver()
    {
        // Make sure the sfxSource is not null and the clip is assigned
        if (gameOverSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(gameOverSound);
        }
    }

    // Helper function to play any sound safely
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}