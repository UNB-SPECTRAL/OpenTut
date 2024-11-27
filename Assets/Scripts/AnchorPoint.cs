// AnchorPoint.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AnchorPoint : MonoBehaviour
{
    public GameObject tutorialMenu;
    private XRGrabInteractable grabInteractable;
    private bool isPlaced = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
        }

        // Hide the tutorial menu initially
        if (tutorialMenu != null)
        {
            tutorialMenu.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the anchor is being held
        if (grabInteractable.isSelected)
        {
            isPlaced = false;
        }
    }

    public void PlaceAnchor()
    {
        if (!isPlaced)
        {
            isPlaced = true;
            
            // Disable further movement
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Show the tutorial menu
            if (tutorialMenu != null)
            {
                tutorialMenu.SetActive(true);
                tutorialMenu.transform.position = transform.position + Vector3.up * 0.5f; // Adjust as needed
            }
        }
    }
}
