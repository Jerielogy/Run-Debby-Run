using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [Header("Main Navigation")]
    public GameObject mainSelectPanel;
    public GameObject luzonPanel;
    public GameObject visayasPanel;
    public GameObject mindanaoPanel;

    [Header("Luzon Configuration")]
    // Drag your pins here in order: Ilocos, CAR, Reg2, Reg3, NCR, 4A, 4B, 5
    public Button[] regionButtons;
    public GameObject[] lockIcons;

    void Start()
    {
        ShowMain();
        UpdateLuzonLocks();
    }

    void UpdateLuzonLocks()
    {
        for (int i = 0; i < regionButtons.Length; i++)
        {
            // 1. Determine if this specific pin is unlocked
            // Element 0 (Ilocos) is unlocked by default (1). Others default to Locked (0).
            int defaultState = (i == 0) ? 1 : 0;
            int isUnlocked = PlayerPrefs.GetInt("RegionUnlocked_" + i, defaultState);

            // 2. Set the Button Interactivity
            if (regionButtons[i] != null)
            {
                regionButtons[i].interactable = (isUnlocked == 1);
            }

            // 3. Set the Lock Icon (WITH SAFETY CHECK)
            // We check "i < lockIcons.Length" to make sure we don't go out of bounds
            if (i < lockIcons.Length && lockIcons[i] != null)
            {
                // If unlocked (1), hide lock. If locked (0), show lock.
                lockIcons[i].SetActive(isUnlocked == 0);
            }
        }
    }

    // --- BUTTON FUNCTIONS ---

    // Call this specifically for Level 1, Level 2, etc.
    // In the Inspector, for the String, type: "Level1", "LevelCAR", "Level3", etc.
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // --- NAVIGATION ---
    public void ShowMain()
    {
        mainSelectPanel.SetActive(true);
        luzonPanel.SetActive(false);
        visayasPanel.SetActive(false);
        mindanaoPanel.SetActive(false);
    }

    public void OpenLuzon() { mainSelectPanel.SetActive(false); luzonPanel.SetActive(true); }
    public void OpenVisayas() { mainSelectPanel.SetActive(false); visayasPanel.SetActive(true); }
    public void OpenMindanao() { mainSelectPanel.SetActive(false); mindanaoPanel.SetActive(true); }


    
}