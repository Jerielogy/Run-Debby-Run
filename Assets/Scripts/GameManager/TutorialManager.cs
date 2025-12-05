using UnityEngine;
using System.Collections;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("Game Objects")]
    public ObstacleSpawner spawner;
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;

    // <--- NEW: The HUD Reference
    public GameObject gameHUD;

    [Header("Prefabs to Spawn")]
    public GameObject rockPrefab;
    public GameObject birdPrefab;

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public float rockY = -2.5f;
    public float birdY = 0.5f;

    private int controlScheme;

    void Start()
    {
        controlScheme = PlayerPrefs.GetInt("ControlScheme", 0);

        // 1. Disable the Spawner (No random obstacles yet)
        if (spawner != null) spawner.enabled = false;

        // 2. Hide the HUD (Score/Level text) immediately
        if (gameHUD != null) gameHUD.SetActive(false);

        // 3. Start the tutorial
        StartCoroutine(RunTutorial());
    }

    IEnumerator RunTutorial()
    {
        // --- WAIT ---
        yield return new WaitForSeconds(2f);

        // --- LESSON 1: JUMP ---
        ShowInstruction("Jump");
        Instantiate(rockPrefab, new Vector3(spawnPoint.position.x, rockY, 0), Quaternion.identity);
        yield return new WaitForSeconds(2.3f);
        instructionPanel.SetActive(false);

        // --- LESSON 2: CROUCH ---
        yield return new WaitForSeconds(6f);
        ShowInstruction("Crouch");
        Instantiate(birdPrefab, new Vector3(spawnPoint.position.x, birdY, 0), Quaternion.identity);
        yield return new WaitForSeconds(4f);
        instructionPanel.SetActive(false);

        // --- TUTORIAL OVER ---
        yield return new WaitForSeconds(2f);

        // 4. SHOW THE HUD (Pop the text onto the screen!)
        if (gameHUD != null) gameHUD.SetActive(true);

        // 5. Enable the Spawner
        if (spawner != null)
        {
            spawner.enabled = true;
        }

        Destroy(gameObject);
    }

    void ShowInstruction(string action)
    {
        instructionPanel.SetActive(true);
        string message = "";

        if (action == "Jump")
        {
            if (controlScheme == 0) message = "Press UP Arrow Key or Space to Jump!";
            if (controlScheme == 1) message = "Say 'JUMP'!";
            if (controlScheme == 2) message = "Press X / A to Jump!";
            if (controlScheme == 3) message = "Tap Right Side!";
        }
        else if (action == "Crouch")
        {
            if (controlScheme == 0) message = "Hold DOWN Arrow to Crouch!";
            if (controlScheme == 1) message = "Say 'CROUCH'!";
            if (controlScheme == 2) message = "Left Stick Down!";
            if (controlScheme == 3) message = "Hold Left Side!";
        }

        instructionText.text = message;
    }
}