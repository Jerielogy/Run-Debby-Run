using UnityEngine;
using UnityEngine.UI;

public class AlbumManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform photoGrid;       // The object with the Grid Layout Group
    public GameObject photoTemplate;  // The Prefab we created

    [Header("Data")]
    public PhotoData[] allPhotos;     // Drag your 18 PhotoData files here


    [Header("Viewer UI")]
    public GameObject photoViewPanel;   // The panel we created
    public Image fullSizeImage;         // The large image inside the panel zoomed
    public Button closeButton;          // The close button
    public Button shareButton;          // The share button
    


    private PhotoData currentOpenPhoto; // Tracks which photo is open

    void OnEnable()
    {
        // Setup the buttons automatically when the game starts
        if (closeButton != null) closeButton.onClick.AddListener(ClosePhotoView);
        if (shareButton != null) shareButton.onClick.AddListener(ShareCurrentPhoto);

        // Hide the view panel at start just in case
        if (photoViewPanel != null) photoViewPanel.SetActive(false);

        LoadAlbum();
    }

    public void LoadAlbum()
    {
        // 1. Clean up old list (if any)
        foreach (Transform child in photoGrid)
        {
            Destroy(child.gameObject);
        }

        // 2. Loop through every photo data file
        foreach (PhotoData photo in allPhotos)
        {
            // Create a new slot in the grid
            GameObject newSlot = Instantiate(photoTemplate, photoGrid);

            // Get components
            Image displayImage = newSlot.GetComponent<Image>();
            Button slotButton = newSlot.GetComponent<Button>();

            // 3. Check if unlocked
            if (photo.IsUnlocked())
            {
                displayImage.sprite = photo.fullImage;

                // NEW: Add click event to open the big viewer
                if (slotButton != null)
                {
                    slotButton.onClick.AddListener(() => OpenPhotoView(photo));
                }
            }
            else
            {
                displayImage.sprite = photo.lockedIcon;

                // NEW: Disable clicking on locked items
                if (slotButton != null) slotButton.interactable = false;
            }
        }
    }


    void OpenPhotoView(PhotoData photo)
    {
        currentOpenPhoto = photo;
        fullSizeImage.sprite = photo.fullImage;
        photoViewPanel.SetActive(true); // Show the popup
    }

    void ClosePhotoView()
    {
        photoViewPanel.SetActive(false); // Hide the popup
    }

    void ShareCurrentPhoto()
    {
        if (currentOpenPhoto == null) return;
        Debug.Log("Sharing photo: " + currentOpenPhoto.photoName);
        // To be add sharing to other socmed platforms later
    }
}