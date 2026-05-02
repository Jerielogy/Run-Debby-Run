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

    private KeywordRecognizer recognizer;
    private Dictionary<string, System.Action> actions = new Dictionary<string, System.Action>();

    // --- HOLD BOTH REFERENCES ---
    private PlayerController player; // For Debby[cite: 1, 2]
    private SwimController swimPlayer; // For Alon

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 1. Flexible Link: Search for whoever is in this region
        player = FindObjectOfType<PlayerController>();
        swimPlayer = FindObjectOfType<SwimController>();

        // 2. Commands for UP/JUMP
        AddCommand("talon", MoveUp);
        AddCommand("jump", MoveUp);
        AddCommand("up", MoveUp);
        AddCommand("angat", MoveUp); // Filipino command for Alon

        // 3. Commands for DOWN/CROUCH[cite: 2]
        AddCommand("yuko", MoveDown);
        AddCommand("crouch", MoveDown);
        AddCommand("down", MoveDown);
        AddCommand("baba", MoveDown); // Filipino command for Alon[cite: 2]

        if (actions.Count > 0)
        {
            recognizer = new KeywordRecognizer(actions.Keys.ToArray(), ConfidenceLevel.Low);
            recognizer.OnPhraseRecognized += OnVoiceDetected;
            recognizer.Start();
            Debug.Log("Voice Active. Accuracy needed: " + requiredAccuracy);
        }
    }

    private void OnVoiceDetected(PhraseRecognizedEventArgs speech)
    {
        float accuracy = GetAccuracyNumber(speech.confidence);
        if (accuracy < requiredAccuracy) return;

        actions[speech.text].Invoke();
    }

    // --- SMART ACTIONS ---
    void MoveUp()
    {
        if (player) player.Jump(); // Debby jumps[cite: 2]
        if (swimPlayer) swimPlayer.ChangeLane(1); // Alon swims up[cite: 2]
    }

    void MoveDown()
    {
        if (player)
        {
            player.Crouch();
            Invoke("StandUp", 1.0f);
        }
        if (swimPlayer) swimPlayer.ChangeLane(-1); // Alon dives down[cite: 2]
    }

    void StandUp() { if (player) player.ReleaseCrouch(); }

    public void StopListening()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.Stop();
            recognizer.Dispose();
        }
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