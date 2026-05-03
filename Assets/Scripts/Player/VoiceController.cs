using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class VoiceController : MonoBehaviour
{
    public static VoiceController Instance;

    [Header("Sensitivity")]
    [Range(0.0f, 1.0f)]
    public float requiredAccuracy = 0.6f;

    [Header("Noise Detection UI")]
    public GameObject noiseWarningPanel;
    [Tooltip("Adjust this based on your room's background noise.")]
    public float noiseThreshold = 0.3f;
    public float noiseCheckInterval = 0.5f;

    private KeywordRecognizer recognizer;
    private Dictionary<string, System.Action> actions = new Dictionary<string, System.Action>();

    // Character References
    private PlayerController player;
    private SwimController swimPlayer;

    // Noise Detection Internals
    private AudioSource micInput;
    private string deviceName;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 1. Link to Characters
        player = FindObjectOfType<PlayerController>();
        swimPlayer = FindObjectOfType<SwimController>();

        // 2. Define Commands
        AddCommand("talon", MoveUp);
        AddCommand("jump", MoveUp);
        AddCommand("up", MoveUp);
        AddCommand("angat", MoveUp);

        AddCommand("yuko", MoveDown);
        AddCommand("crouch", MoveDown);
        AddCommand("slide", MoveDown);
        AddCommand("down", MoveDown);
        AddCommand("baba", MoveDown);

        // 3. Start Voice Recognition
        if (actions.Count > 0)
        {
            recognizer = new KeywordRecognizer(actions.Keys.ToArray(), ConfidenceLevel.Low);
            recognizer.OnPhraseRecognized += OnVoiceDetected;
            recognizer.Start();
        }

        // 4. Initialize Microphone for Noise Monitoring
        if (Microphone.devices.Length > 0)
        {
            deviceName = Microphone.devices[0];
            micInput = gameObject.AddComponent<AudioSource>();
            micInput.clip = Microphone.Start(deviceName, true, 10, 44100);
            micInput.loop = true;
            micInput.mute = true; // Mute to prevent audio feedback loops
            while (!(Microphone.GetPosition(deviceName) > 0)) { }
            micInput.Play();

            // Check noise levels repeatedly
            InvokeRepeating("CheckNoiseLevel", 0f, noiseCheckInterval);
        }
    }

    private void OnVoiceDetected(PhraseRecognizedEventArgs speech)
    {
        float accuracy = GetAccuracyNumber(speech.confidence);
        if (accuracy < requiredAccuracy) return;

        actions[speech.text].Invoke();
    }

    // --- NOISE MONITORING LOGIC ---
    void CheckNoiseLevel()
    {
        float currentLevel = GetLoudness();

        if (noiseWarningPanel != null)
        {
            // Toggle panel if the environment is too loud for reliable recognition
            noiseWarningPanel.SetActive(currentLevel > noiseThreshold);
        }
    }

    float GetLoudness()
    {
        float[] waveData = new float[128];
        int micPosition = Microphone.GetPosition(deviceName) - 128;
        if (micPosition < 0) return 0;

        micInput.clip.GetData(waveData, micPosition);

        float totalLoudness = 0;
        for (int i = 0; i < 128; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }
        return totalLoudness / 128;
    }

    // --- MOVEMENT ACTIONS ---
    void MoveUp()
    {
        if (player) player.Jump();
        if (swimPlayer) swimPlayer.ChangeLane(1);
    }

    void MoveDown()
    {
        if (player)
        {
            player.Crouch();
            Invoke("StandUp", 1.0f);
        }
        if (swimPlayer) swimPlayer.ChangeLane(-1);
    }

    void StandUp() { if (player) player.ReleaseCrouch(); }

    // --- SYSTEM CONTROLS ---
    public void StopListening()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.Stop();
            recognizer.Dispose();
        }
        Microphone.End(deviceName);
    }

    void AddCommand(string word, System.Action method)
    {
        if (!actions.ContainsKey(word)) actions.Add(word, method);
    }

    float GetAccuracyNumber(ConfidenceLevel level)
    {
        if (level == ConfidenceLevel.High) return 0.9f;
        if (level == ConfidenceLevel.Medium) return 0.7f;
        return 0.5f;
    }

    void OnDestroy()
    {
        StopListening();
    }
}