using UnityEngine;
using TMPro;

public class KnowledgeRewardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject knowledgePanel;       // The pop-out window
    public TextMeshProUGUI knowledgeText;   // The text area inside the pop-out

    [Header("Region Data")]
    public string regionTitle;              // e.g., "Region 6: Western Visayas"
    [TextArea(5, 10)]
    public string information;              // The "Good Information" about the region

    void Start()
    {
        // Ensure the panel is hidden when the level starts
        if (knowledgePanel != null)
            knowledgePanel.SetActive(false);
    }

    // This function will be called by your "View Info" button
    public void OpenKnowledgePanel()
    {
        if (knowledgePanel != null && knowledgeText != null)
        {
            // Update the text before showing the panel
            knowledgeText.text = "<b>" + regionTitle + "</b>\n\n" + information;
            knowledgePanel.SetActive(true);
        }
    }

    // This function will be called by a "Close" button on the pop-out
    public void CloseKnowledgePanel()
    {
        if (knowledgePanel != null)
            knowledgePanel.SetActive(false);
    }
}