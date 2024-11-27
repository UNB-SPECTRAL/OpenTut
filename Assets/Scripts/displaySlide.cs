using UnityEngine;
using UnityEngine.UI;

public class displaySlide : MonoBehaviour
{
    public RawImage slideImage;
    public Texture2D[] slides;
    private int currentSlide = 0;

    void Start()
    {
        if (slides.Length > 0)
        {
            slideImage.texture = slides[0];
            Debug.Log("On slide 1");
        }
    }

    public void NextSlide()
{
    Debug.Log("NextSlide method entered");
    if (currentSlide < slides.Length - 1)
    {
        currentSlide++;
        Debug.Log($"Moving to slide { currentSlide}");
        if (slideImage != null && slides[currentSlide] != null)
        {
            slideImage.texture = slides[currentSlide];
            Debug.Log("Slide image updated");
        }
        else
        {
            Debug.LogError("slideImage or slide texture is null");
        }
    }
    else
    {
        Debug.Log("Already at last slide");
    }
}

public void PreviousSlide()
{
    Debug.Log("PreviousSlide method entered");
    if (currentSlide > 0)
    {
        currentSlide--;
        Debug.Log($"Moving to slide { currentSlide}");
        if (slideImage != null && slides[currentSlide] != null)
        {
            slideImage.texture = slides[currentSlide];
            Debug.Log("Slide image updated");
        }
        else
        {
            Debug.LogError("slideImage or slide texture is null");
        }
    }
    else
    {
        Debug.Log("Already at first slide");
    }
}
}
