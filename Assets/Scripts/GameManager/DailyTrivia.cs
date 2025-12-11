using UnityEngine;
using TMPro;
using System; // For DateTime

public class DailyTrivia : MonoBehaviour
{
    [Header("UI References")]
    public GameObject triviaPanel;
    public TextMeshProUGUI factText;

    [Header("Content")]
    [TextArea] public string[] funFacts; // Write your list here in the Inspector!

    private const string TRIVIA_DATE_KEY = "LastTriviaDate";
    public bool debugMode = true; // Check this to test freely

    public void ShowDailyTrivia()
    {
        // 1. Check Date (Optional: prevent spamming)
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        string lastPlayed = PlayerPrefs.GetString(TRIVIA_DATE_KEY, "");

        if (!debugMode && lastPlayed == today)
        {
            // If they already saw it, maybe show a "Come back tomorrow" message?
            // For now, let's just show the same fact again or return.
            Debug.Log("Already showed trivia today.");
        }

        // 2. Pick a Random Fact
        if (funFacts.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, funFacts.Length);
            factText.text = funFacts[randomIndex];
        }
        else
        {
            factText.text = "No facts loaded yet!";
        }

        // 3. Open Panel
        triviaPanel.SetActive(true);

        // 4. Save Date
        PlayerPrefs.SetString(TRIVIA_DATE_KEY, today);
    }

    public void ClosePanel()
    {
        triviaPanel.SetActive(false);
    }
}