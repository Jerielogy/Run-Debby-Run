using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MasterMapManager : MonoBehaviour
{
    [Header("Intro & Dialogue")]
    public GameObject panelIntroPH;
    public TextMeshProUGUI introText;
    public TextMeshProUGUI introButtonText;
    [TextArea(3, 10)]
    public string[] introDialogues;
    private int dialogueIndex = 0;

    public GameObject[] guideHands;

    [Header("Main Panels")]
    public GameObject panelMainSelect;
    public GameObject panelLuzonMap;
    public GameObject panelVisayasMap;
    public GameObject panelMindanaoMap;

    [Header("Level Preview Card")]
    public GameObject previewCard;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    private string sceneToLoad;

    [Header("Region Buttons (Main Select)")]
    public Button luzonBtn;
    public Button visayasBtn;
    public Button mindanaoBtn;

    [Header("Luzon Level Pins")]
    [Tooltip("Order: Region 1, CAR, Region 2, Region 3, Region NCR, Region 4A, Region 4B, Region 5")]
    public Button[] luzonLevelPins;

    [Header("Luzon Progress")]
    public Image luzonMapDisplay;
    public Sprite[] luzonFrames;

    [Header("Visayas Progress")]
    public Image visayasMapDisplay;
    public Sprite[] visayasFrames;

    [Header("Mindanao Progress")]
    public Image mindanaoMapDisplay;
    public Sprite[] mindanaoFrames;

    void Start()
    {
        Time.timeScale = 1f;

        // Only show intro if it's a fresh game
        if (PlayerPrefs.GetInt("IntroPlayed", 0) == 0)
        {
            StartIntro();
        }
        else
        {
            panelIntroPH.SetActive(false);
            ShowMainMap();
        }
    }

    // --- INTRO DIALOGUE LOGIC ---

    public void StartIntro()
    {
        dialogueIndex = 0;
        panelIntroPH.SetActive(true);
        panelMainSelect.SetActive(true);
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
        PlayerPrefs.SetInt("IntroPlayed", 1);
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
        previewCard.SetActive(false);

        UpdateRegionUnlocks();
    }

    private void UpdateRegionUnlocks()
    {
        // FIX: Load the actual saved progress instead of the hardcoded '3'
        int worldProgress = PlayerPrefs.GetInt("WorldProgress", 1);

        luzonBtn.interactable = true;
        visayasBtn.interactable = (worldProgress >= 2);
        mindanaoBtn.interactable = (worldProgress >= 3);

        // Update Hands
        if (guideHands.Length > 0 && guideHands[0] != null)
            guideHands[0].SetActive(true);

        if (guideHands.Length > 1 && guideHands[1] != null)
            guideHands[1].SetActive(worldProgress >= 2);

        if (guideHands.Length > 2 && guideHands[2] != null)
            guideHands[2].SetActive(worldProgress >= 3);
    }

    public void OpenLuzon()
    {
        HideAllHands();
        panelMainSelect.SetActive(false);
        panelLuzonMap.SetActive(true);

        int progress = PlayerPrefs.GetInt("LuzonProgress", 0);

        // UPDATE: Sequential Pin Unlocking
        for (int i = 0; i < luzonLevelPins.Length; i++)
        {
            if (luzonLevelPins[i] != null)
            {
                // Level 0 (Region 1) is always true. 
                // Subsequent levels unlock if progress is greater than or equal to their index.
                luzonLevelPins[i].interactable = (progress >= i);
            }
        }

        if (progress < luzonFrames.Length && luzonMapDisplay != null)
        {
            luzonMapDisplay.sprite = luzonFrames[progress];
        }
    }

    // Visayas and Mindanao functions remain the same...
    public void OpenVisayas() { /* Logic same as OpenLuzon but for Visayas vars */ }
    public void OpenMindanao() { /* Logic same as OpenLuzon but for Mindanao vars */ }

    // --- LEVEL PREVIEW CARD ---
    public void OpenPreview(string levelName, string levelDescription, string targetScene)
    {
        titleText.text = levelName;
        descText.text = levelDescription;
        sceneToLoad = targetScene;
        previewCard.SetActive(true);
    }

    public void ClosePreview() { previewCard.SetActive(false); }

    public void PlaySelectedLevel()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // --- UTILITY ---
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu() { SceneManager.LoadScene("MainMenu"); }

    private void HideAllHands()
    {
        foreach (GameObject hand in guideHands)
        {
            if (hand != null) hand.SetActive(false);
        }
    }
}