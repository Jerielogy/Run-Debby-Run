using UnityEngine;
using System.IO;
using TMPro; // Required for the "Saved!" message

public class PCPhotoManager : MonoBehaviour
{
    private string folderName = "RunDebbyRun_Photos";

    [Header("UI Feedback")]
    public GameObject savedNotification; // A small panel or text that says "Photo Saved!"

    public void SaveAndOpenSocial(Sprite photoSprite)
    {
        if (photoSprite == null) return;

        // 1. Get the path to "My Pictures"
        string picturesPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
        string finalPath = Path.Combine(picturesPath, folderName);
        if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);

        // 2. Prepare the texture (Safe version to avoid compression errors)
        Texture2D sourceTexture = photoSprite.texture;
        Texture2D readableText = new Texture2D(sourceTexture.width, sourceTexture.height);

        RenderTexture tmp = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height, 0);
        Graphics.Blit(sourceTexture, tmp);
        RenderTexture.active = tmp;
        readableText.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableText.Apply();
        RenderTexture.ReleaseTemporary(tmp);

        // 3. Save the file
        byte[] bytes = readableText.EncodeToPNG();
        string fileName = "Debby_Adventure_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string fullPath = Path.Combine(finalPath, fileName);
        File.WriteAllBytes(fullPath, bytes);

        // 4. THE "PRO" ADDITIONS:
        // Copy a message to the clipboard so they can just Ctrl+V on Facebook
        GUIUtility.systemCopyBuffer = "Look at what I unlocked in Run Debby Run!";

        // Show the in-game notification
        if (savedNotification != null) StartCoroutine(ShowNotification());

        // 5. Open the folder and browser
        Application.OpenURL("file://" + finalPath);
        Application.OpenURL("https://www.facebook.com");

        Destroy(readableText);
    }

    private System.Collections.IEnumerator ShowNotification()
    {
        savedNotification.SetActive(true);
        yield return new WaitForSeconds(3f); // Show for 3 seconds
        savedNotification.SetActive(false);
    }
}