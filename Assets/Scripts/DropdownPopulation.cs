using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class DropdownPopulator : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown slideDropdown;
    [SerializeField] private TMP_Text errorText;

    private const string UNITY_SLIDES_PATH = "Assets/Slides";
    private string[] folderPaths;

    void Start()
    {
        Debug.Log("DropdownPopulator Start");
        // Wait a frame to ensure slides are imported
        Invoke("PopulateDropdown", 0.1f);
    }

    public void PopulateDropdown()
    {
        Debug.Log("Starting to populate dropdown");
        
        // Clear existing options
        slideDropdown.ClearOptions();
        List<string> options = new List<string>();
        
        // Add default option
        options.Add("Select Slide Deck");

        if (!Directory.Exists(UNITY_SLIDES_PATH))
        {
            Debug.LogError($"Slides folder not found at: { UNITY_SLIDES_PATH}");
            slideDropdown.AddOptions(options);
            return;
        }

        // Get all folders in the Slides directory
        folderPaths = Directory.GetDirectories(UNITY_SLIDES_PATH);
        Debug.Log($"Found { folderPaths.Length} folders in {UNITY_SLIDES_PATH}");

        foreach (string folderPath in folderPaths)
        {
            string folderName = Path.GetFileName(folderPath);
            // Get everything after the first dash
            int dashIndex = folderName.IndexOf("-");
            if (dashIndex > 0 && dashIndex + 1 < folderName.Length)
            {
                folderName = folderName.Substring(dashIndex + 1).Trim();
            }
            
            Debug.Log($"Adding option: { folderName}");
            options.Add(folderName);
        }

        // Add all options to dropdown
        slideDropdown.AddOptions(options);
        Debug.Log($"Added { options.Count} options to dropdown");
    }

    public void RefreshDropdown()
    {
        Debug.Log("Refreshing Dropdown");
        PopulateDropdown();
    }

    public string GetSelectedFolderPath()
    {
        if (slideDropdown.value == 0 || folderPaths == null || folderPaths.Length == 0)
            return null;
            
        return folderPaths[slideDropdown.value - 1];
    }

    // Helper method to print all folders in the Slides directory
    private void DebugPrintFolders()
    {
        if (Directory.Exists(UNITY_SLIDES_PATH))
        {
            string[] folders = Directory.GetDirectories(UNITY_SLIDES_PATH);
            Debug.Log("Folders in Slides directory:");
            foreach (string folder in folders)
            {
                Debug.Log(Path.GetFileName(folder));
            }
        }
        else
        {
            Debug.LogError("Slides directory does not exist");
        }
    }
}