using UnityEngine;

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
        PlaySFX(gameOverSound);
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