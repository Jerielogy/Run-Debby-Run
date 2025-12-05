using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class VoiceController : MonoBehaviour
{
    // Singleton Access (Optional, but helps GameManager find it)
    public static VoiceController Instance;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> actions = new Dictionary<string, System.Action>();
    private PlayerController playerController;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // --- 1. THE GATEKEEPER ---
        // 0 = Keyboard, 1 = Voice, 2 = Joystick, 3 = Touch
        int currentScheme = PlayerPrefs.GetInt("ControlScheme", 0);

        // If NOT Voice (1), shut down.
        if (currentScheme != 1)
        {
            this.enabled = false;
            return;
        }
        // -------------------------

        playerController = FindObjectOfType<PlayerController>();

        // Define words
        actions.Add("jump", Jump);
        actions.Add("up", Jump);
        actions.Add("fly", Jump);

        actions.Add("crouch", Crouch);
        actions.Add("down", Crouch);
        actions.Add("duck", Crouch);

        // Start listening
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();

        Debug.Log("Voice Control Active");
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        actions[speech.text].Invoke();
    }

    private void Jump()
    {
        if (playerController != null) playerController.Jump();
    }

    private void Crouch()
    {
        if (playerController != null) playerController.Crouch();
    }

    // --- THE MISSING METHOD ---
    public void StopListening()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
            Debug.Log("Voice Control Stopped");
        }
    }

    // Ensure we clean up if the object is destroyed
    void OnDestroy()
    {
        StopListening();
    }
}