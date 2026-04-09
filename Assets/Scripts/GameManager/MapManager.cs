using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MasterMapManager : MonoBehaviour
{
    [Header("Intro & Dialogue")]
    public GameObject panelIntroPH;
    public TextMeshProUGUI introText;
    public TextMeshProUGUI introButtonText; // To change "Next" to "Let's Go!"
    [TextArea(3, 10)]
    public string[] introDialogues; // Put your 3 parts here in the Inspector
    private int dialogueIndex = 0;

    [Header("Main Panels")]
    public GameObject panelMainSelect;
    public GameObject panelLuzonMap;
    public GameObject panelVisayasMap;
    public GameObject panelMindanaoMap;

    [Header("Level Preview Card")]
    public GameObject previewCard;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    private string sceneToLoad; // Stores the name of the level to play

    [Header("Region Buttons (Main Select)")]
    public Button luzonBtn;
    public Button visayasBtn;
    public Button mindanaoBtn;

    [Header("Luzon Progress")]
    public Image luzonMapDisplay;
    public Sprite[] luzonFrames;

    void Start()
    {
        Time.timeScale = 1f;
        StartIntro();
    }

    // --- INTRO DIALOGUE LOGIC ---

    public void StartIntro()
    {
        dialogueIndex = 0;
        panelIntroPH.SetActive(true);
        panelMainSelect.SetActive(true); // Keep map visible in background
        UpdateDialogueUI();
    }

    public void AdvanceDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex < introDialogues.Length)
        {
            UpdateDialogueUI();
        }
        else
        {
            CloseIntro();
        }
    }

    private void UpdateDialogueUI()
    {
        introText.text = introDialogues[dialogueIndex];

        if (introButtonText != null)
        {
            introButtonText.text = (dialogueIndex == introDialogues.Length - 1) ? "Let's Go!" : "Next";
        }
    }

    public void CloseIntro()
    {
        panelIntroPH.SetActive(false);
        ShowMainMap();
    }

    // --- NAVIGATION ---

    public void ShowMainMap()
    {
        panelMainSelect.SetActive(true);
        panelLuzonMap.SetActive(false);
        panelVisayasMap.SetActive(false);
        panelMindanaoMap.SetActive(false);
        previewCard.SetActive(false); // Close any open cards

        UpdateRegionUnlocks();
    }

    private void UpdateRegionUnlocks()
    {
        int worldProgress = PlayerPrefs.GetInt("WorldProgress", 1);
        luzonBtn.interactable = true;
        visayasBtn.interactable = (worldProgress >= 2);
        mindanaoBtn.interactable = (worldProgress >= 3);
    }

    public void OpenLuzon()
    {
        panelMainSelect.SetActive(false);
        panelLuzonMap.SetActive(true);

        int progress = PlayerPrefs.GetInt("LuzonProgress", 0);
        if (progress < luzonFrames.Length && luzonMapDisplay != null)
        {
            luzonMapDisplay.sprite = luzonFrames[progress];
        }
    }

    // --- LEVEL PREVIEW CARD LOGIC ---

    public void OpenPreview(string levelName, string levelDescription, string targetScene)
    {
        titleText.text = levelName;
        descText.text = levelDescription;
        sceneToLoad = targetScene;

        previewCard.SetActive(true);
    }

    public void ClosePreview()
    {
        previewCard.SetActive(false);
    }

    public void PlaySelectedLevel()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Loading Scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // --- UTILITY ---

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        StartIntro();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}