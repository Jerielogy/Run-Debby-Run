using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class VoiceController : MonoBehaviour
{
    public static VoiceController Instance; // Needed for other scripts to find this

    // --- SETTINGS ---
    [Header("Sensitivity")]
    [Tooltip("Higher = Strict. Lower = Easy.")]
    [Range(0.0f, 1.0f)]
    public float requiredAccuracy = 0.6f;

    // --- INTERNAL VARIABLES ---
    private KeywordRecognizer recognizer;
    private Dictionary<string, System.Action> actions = new Dictionary<string, System.Action>();
    private PlayerController player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 1. Link to Player
        player = FindObjectOfType<PlayerController>();

        // 2. Define Commands (Filipino & English)
        AddCommand("talon", Jump);
        AddCommand("jump", Jump);
        AddCommand("up", Jump);

        AddCommand("yuko", Crouch);
        AddCommand("crouch", Crouch);
        AddCommand("down", Crouch);

        // 3. Start Listening 
        if (actions.Count > 0)
        {
            recognizer = new KeywordRecognizer(actions.Keys.ToArray(), ConfidenceLevel.Low);
            recognizer.OnPhraseRecognized += OnVoiceDetected;
            recognizer.Start();
            Debug.Log("Voice Active. Accuracy needed: " + requiredAccuracy);
        }
    }

    // --- THE BRAIN ---
    private void OnVoiceDetected(PhraseRecognizedEventArgs speech)
    {
        float accuracy = GetAccuracyNumber(speech.confidence);
        Debug.Log($"Heard: '{speech.text}' (Accuracy: {accuracy})");

        if (accuracy < requiredAccuracy) return; // Ignore if mumbled

        actions[speech.text].Invoke(); // Do the action
    }

    // --- ACTIONS ---
    void Jump() { if (player) player.Jump(); }
    void Crouch()
    {
        if (player)
        {
            player.Crouch();
            Invoke("StandUp", 1.0f);
        }
    }
    void StandUp() { if (player) player.ReleaseCrouch(); }

    // --- THE MISSING FUNCTION (Added this back!) ---
    public void StopListening()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.Stop();
            recognizer.Dispose();
            Debug.Log("Voice Control Stopped.");
        }
    }

    // --- HELPERS ---
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