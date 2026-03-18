using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject[] panels; // Drag Panel_1, Panel_2, etc. here in order
    public GameObject gameHUD;  // Drag your HUD (with the Score Text) here

    private int currentPanelIndex = 0;

    void Start()
    {
        Time.timeScale = 0f; // Freeze game at start
        if (gameHUD != null) gameHUD.SetActive(false); // Hide Score until Start
        ShowCurrentPanel();
    }

    public void NextPanel()
    {
        // Cycle to next panel if we aren't at the end
        if (currentPanelIndex < panels.Length - 1)
        {
            currentPanelIndex++;
            ShowCurrentPanel();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f; // Unfreeze game
        if (gameHUD != null) gameHUD.SetActive(true); // Show Score UI
        gameObject.SetActive(false); // Hide the entire Intro UI
    }

    private void ShowCurrentPanel()
    {
        // Simply turns off all panels and turns on ONLY the current index
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
                panels[i].SetActive(i == currentPanelIndex);
        }
    }
}