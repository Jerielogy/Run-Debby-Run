using UnityEngine;

public class LevelPin : MonoBehaviour
{
    [Header("Level Information")]
    public string regionName;
    [TextArea(3, 5)]
    public string levelDescription;
    public string sceneToLoad;

    // This function will talk to your MasterMapManager
    public void ClickPin()
    {
        // Find the Manager in the scene and pass the data
        MasterMapManager manager = FindObjectOfType<MasterMapManager>();

        if (manager != null)
        {
            manager.OpenPreview(regionName, levelDescription, sceneToLoad);
        }
        else
        {
            Debug.LogError("MasterMapManager not found in the scene!");
        }
    }
}