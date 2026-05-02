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

    [Header("Main Selection Buttons")]
    public Button luzonBtn;
    public Button visayasBtn;
    public Button mindanaoBtn;

    [Header("Regional Level Pins")]
    public Button[] luzonLevelPins;
    public Button[] visayasLevelPins;
    public Button[] mindanaoLevelPins;

    [Header("Regional Progress Displays")]
    public Image luzonMapDisplay;
    public Sprite[] luzonFrames;
    public Image visayasMapDisplay;
    public Sprite[] visayasFrames;
    public Image mindanaoMapDisplay;
    public Sprite[] mindanaoFrames;

    void Start()
    {
        Time.timeScale = 1f;

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

    // --- NAVIGATION & PROGRESSION ---

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
        // 1. Get exact region progress[cite: 2]
        int luzonProgress = PlayerPrefs.GetInt("LuzonProgress", 0);
        int visayasProgress = PlayerPrefs.GetInt("VisayasProgress", 0);
        int worldProgress = PlayerPrefs.GetInt("WorldProgress", 1);

        // 2. Hide or Show Buttons based on specific requirements[cite: 2]
        luzonBtn.gameObject.SetActive(true);

        // Visayas stays hidden until Region 5 is done[cite: 2]
        bool isVisayasUnlocked = (luzonProgress >= 5 || worldProgress >= 2);
        visayasBtn.gameObject.SetActive(isVisayasUnlocked);

        // Mindanao stays hidden until Region 8 is done[cite: 2]
        bool isMindanaoUnlocked = (visayasProgress >= 8 || worldProgress >= 3);
        mindanaoBtn.gameObject.SetActive(isMindanaoUnlocked);
    }

    public void OpenLuzon() { SetupRegionalMap(panelLuzonMap, "LuzonProgress", luzonLevelPins, luzonMapDisplay, luzonFrames); }
    public void OpenVisayas() { SetupRegionalMap(panelVisayasMap, "VisayasProgress", visayasLevelPins, visayasMapDisplay, visayasFrames); }
    public void OpenMindanao() { SetupRegionalMap(panelMindanaoMap, "MindanaoProgress", mindanaoLevelPins, mindanaoMapDisplay, mindanaoFrames); }

    private void SetupRegionalMap(GameObject panel, string prefKey, Button[] pins, Image display, Sprite[] frames)
    {
        panelMainSelect.SetActive(false);
        panel.SetActive(true);

        int progress = PlayerPrefs.GetInt(prefKey, 0);

        for (int i = 0; i < pins.Length; i++)
        {
            if (pins[i] != null) pins[i].interactable = (progress >= i);
        }

        if (display != null && frames != null && progress < frames.Length)
        {
            display.sprite = frames[progress];
        }
    }

    // --- INTRO & DIALOGUE ---

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
        if (dialogueIndex < introDialogues.Length) UpdateDialogueUI();
        else CloseIntro();
    }

    private void UpdateDialogueUI()
    {
        introText.text = introDialogues[dialogueIndex];
        if (introButtonText != null)
            introButtonText.text = (dialogueIndex == introDialogues.Length - 1) ? "Let's Go!" : "Next";
    }

    public void CloseIntro()
    {
        PlayerPrefs.SetInt("IntroPlayed", 1);
        panelIntroPH.SetActive(false);
        ShowMainMap();
    }

    // --- PREVIEW & UTILITY ---

    public void OpenPreview(string name, string desc, string scene)
    {
        titleText.text = name;
        descText.text = desc;
        sceneToLoad = scene;
        previewCard.SetActive(true);
    }

    public void ClosePreview() { previewCard.SetActive(false); }

    public void PlaySelectedLevel()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneTransitionManager.Instance.LoadScene(sceneToLoad);
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu() { SceneTransitionManager.Instance.LoadScene("MainMenu"); }
}