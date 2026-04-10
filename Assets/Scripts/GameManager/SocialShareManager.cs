using UnityEngine;

public class SocialShareManager : MonoBehaviour
{
    [Header("Share Settings")]
    public string gameLink = "https://your-game-link.com"; // Link to your itch.io or project page
    public string defaultMessage = "Check out this photo I unlocked in my adventure in Run, Debby Run!";

    // --- FACEBOOK SHARING ---
    public void ShareOnFacebook()
    {
        // Facebook uses a specific URL format to share links
        string facebookURL = "https://www.facebook.com/sharer/sharer.php?u=" + System.Uri.EscapeDataString(gameLink);
        Application.OpenURL(facebookURL);
    }

    // --- TWITTER (X) SHARING ---
    public void ShareOnTwitter()
    {
        // Twitter allows you to include both text and a link
        string twitterURL = "https://twitter.com/intent/tweet?text=" + System.Uri.EscapeDataString(defaultMessage) + "&url=" + System.Uri.EscapeDataString(gameLink);
        Application.OpenURL(twitterURL);
    }
}