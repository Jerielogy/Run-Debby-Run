using UnityEngine;

[CreateAssetMenu(fileName = "New Photo", menuName = "Rewards/Photo")]
public class PhotoData : ScriptableObject
{
    [Header("Photo Info")]
    public string photoName;     // <--- ADD THIS LINE HERE!
    public int photoID;
    public string regionName;

    [Header("Visuals")]
    public Sprite fullImage;
    public Sprite lockedIcon;

    // ... keep the rest of your Save/Unlock code below ...
    // Generate a unique key for saving (e.g., "Photo_1_Unlocked")
    private string GetSaveKey()
    {
        return "Photo_" + photoID + "_Unlocked";
    }

    public bool IsUnlocked()
    {
        return PlayerPrefs.GetInt(GetSaveKey(), 0) == 1;
    }

    public void Unlock()
    {
        PlayerPrefs.SetInt(GetSaveKey(), 1);
        PlayerPrefs.Save();
    }
}