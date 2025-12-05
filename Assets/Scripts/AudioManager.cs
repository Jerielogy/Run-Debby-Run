using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton so we can call it from anywhere

    [Header("Audio Sources")]
    public AudioSource musicSource; // For BGM (Loops)
    public AudioSource sfxSource;   // For SFX (One-shot sounds)

    [Header("Sound Clips")]
    public AudioClip backgroundMusic;
    public AudioClip jumpSound;
    public AudioClip crouchSound;

    // You can add more later:
    // public AudioClip scoreSound;
    // public AudioClip crashSound;

    void Awake()
    {
        // Singleton Setup (Like SceneTransitionManager)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this alive across levels
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate if we go back to menu
        }
    }

    void Start()
    {
        // Start BGM automatically
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // Call this to play simple SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Specific helpers for easy calling
    public void PlayJump() => PlaySFX(jumpSound);
    public void PlayCrouch() => PlaySFX(crouchSound);
}