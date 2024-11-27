using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SlideManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject inputCodePanel;
    [SerializeField] private GameObject slidePanelMain;

    [Header("Components")]
    [SerializeField] private DropdownPopulator dropdownPopulator;
    [SerializeField] private Button confirmButton;
    [SerializeField] private displaySlide slideDisplayScript;

    private const string UNITY_SLIDES_PATH = "Assets/Slides";
    private const string LOCAL_SLIDES_PATH = "C:/Users/acui6/Downloads/Slide_Deck_Folder";

    void Start()
    {
        Debug.Log("SlideManager Start");
        
        // Verify references
        if (inputCodePanel == null) Debug.LogError("inputCodePanel is not assigned!");
        if (slidePanelMain == null) Debug.LogError("slidePanelMain is not assigned!");
        if (dropdownPopulator == null) Debug.LogError("dropdownPopulator is not assigned!");
        if (confirmButton == null) Debug.LogError("confirmButton is not assigned!");
        if (slideDisplayScript == null) Debug.LogError("slideDisplayScript is not assigned!");

        inputCodePanel.SetActive(true);
        slidePanelMain.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        // Ensure Unity Slides folder exists
        if (!Directory.Exists(UNITY_SLIDES_PATH))
        {
            Directory.CreateDirectory(UNITY_SLIDES_PATH);
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
        }

        // Import all folders from local path
        ImportAllSlideFolders();
    }

    private void ImportAllSlideFolders()
    {
        if (!Directory.Exists(LOCAL_SLIDES_PATH))
        {
            Debug.LogError($"Local slides folder not found at: { LOCAL_SLIDES_PATH}");
            return;
        }

        // Get all directories from local folder
        string[] localFolders = Directory.GetDirectories(LOCAL_SLIDES_PATH);
        Debug.Log($"Found { localFolders.Length} folders to import");

        foreach (string sourceFolder in localFolders)
        {
            string folderName = Path.GetFileName(sourceFolder);
            string unityFolderPath = Path.Combine(UNITY_SLIDES_PATH, folderName);

            // Create folder in Unity's Assets/Slides
            if (!Directory.Exists(unityFolderPath))
            {
                Directory.CreateDirectory(unityFolderPath);
                Debug.Log($"Created folder: { folderName}");
            }

            // Get all images from source folder
            string[] imageFiles = Directory.GetFiles(sourceFolder)
                .Where(file => file.ToLower().EndsWith(".jpg") || 
                              file.ToLower().EndsWith(".jpeg") || 
                              file.ToLower().EndsWith(".png"))
                .ToArray();

            // Import each image
            foreach (string imageFile in imageFiles)
            {
                string fileName = Path.GetFileName(imageFile);
                string destPath = Path.Combine(unityFolderPath, fileName);

                try
                {
                    // Copy file if it doesn't exist or if source is newer
                    if (!File.Exists(destPath) || 
                        File.GetLastWriteTime(imageFile) > File.GetLastWriteTime(destPath))
                    {
                        File.Copy(imageFile, destPath, true);
                        Debug.Log($"Imported: { fileName} to {folderName}");

                        #if UNITY_EDITOR
                        AssetDatabase.ImportAsset(destPath.Replace('\\', '/'));
                        #endif
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error importing { fileName}: {e.Message}");
                }
            }
        }

        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif

        Debug.Log("Finished importing all slide folders");

        // Refresh the dropdown after importing
        if (dropdownPopulator != null)
        {
            dropdownPopulator.RefreshDropdown();
        }
    }

    private void OnConfirmButtonClicked()
    {
        Debug.Log("Confirm button clicked");
        string selectedFolderPath = dropdownPopulator.GetSelectedFolderPath();
        
        if (string.IsNullOrEmpty(selectedFolderPath))
        {
            Debug.LogWarning("No slide deck selected");
            return;
        }

        Debug.Log($"Selected folder path: { selectedFolderPath}");
        LoadSelectedSlides(selectedFolderPath);
    }

    private void LoadSelectedSlides(string folderPath)
    {
        Debug.Log($"Loading slides from folder: { folderPath}");

        // Get all images from the selected folder
        string[] imageFiles = Directory.GetFiles(folderPath)
            .Where(file => file.ToLower().EndsWith(".jpg") || 
                          file.ToLower().EndsWith(".jpeg") || 
                          file.ToLower().EndsWith(".png"))
            .OrderBy(file => file)
            .ToArray();

        Debug.Log($"Found { imageFiles.Length} images in folder");

        // Create array for textures
        Texture2D[] slides = new Texture2D[imageFiles.Length];

        // Load each image
        for (int i = 0; i < imageFiles.Length; i++)
        {
            byte[] imageData = File.ReadAllBytes(imageFiles[i]);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageData);
            slides[i] = tex;
            Debug.Log($"Loaded slide { i + 1} of {imageFiles.Length}");
        }

        // Verify slideDisplayScript is assigned
        if (slideDisplayScript == null)
        {
            Debug.LogError("slideDisplayScript is not assigned!");
            return;
        }

        // Assign slides to displaySlide script
        slideDisplayScript.slides = slides;
        Debug.Log($"Assigned { slides.Length} slides to displaySlide script");

        ShowSlidePanel();
    }

    private void ShowSlidePanel()
    {
        Debug.Log("ShowSlidePanel called");
        Debug.Log($"Before switch - InputCodePanel active: { inputCodePanel.activeSelf}, SlidePanelMain active: {slidePanelMain.activeSelf}");
        
        // Verify panel references
        if (inputCodePanel == null || slidePanelMain == null)
        {
            Debug.LogError("Panel references are missing!");
            return;
        }

        // Check parent Canvas
        Canvas canvas = slidePanelMain.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in parents!");
            return;
        }

        // Check RectTransform
        RectTransform panelRect = slidePanelMain.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            Debug.LogError("No RectTransform found on slidePanelMain!");
            return;
        }

        inputCodePanel.SetActive(false);
        slidePanelMain.SetActive(true);

        Debug.Log($"After switch - InputCodePanel active: { inputCodePanel.activeSelf}, SlidePanelMain active: {slidePanelMain.activeSelf}");
        Debug.Log($"Panel position: { panelRect.position}, size: {panelRect.rect.size}");
        
        // Check if any CanvasGroup might be affecting visibility
        CanvasGroup canvasGroup = slidePanelMain.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            Debug.Log($"CanvasGroup found - Alpha: { canvasGroup.alpha}, Interactable: {canvasGroup.interactable}, BlocksRaycasts: {canvasGroup.blocksRaycasts}");
        }

        // Force canvas update
        Canvas.ForceUpdateCanvases();
        
        // Check hierarchy path
        Transform current = slidePanelMain.transform;
        string hierarchyPath = current.name;
        while (current.parent != null)
        {
            current = current.parent;
            hierarchyPath = current.name + "/" + hierarchyPath;
        }
        Debug.Log($"SlidePanelMain hierarchy path: { hierarchyPath}");
    }

    public void ReturnToSelection()
    {
        Debug.Log("Returning to selection");
        slidePanelMain.SetActive(false);
        inputCodePanel.SetActive(true);
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
    }
}