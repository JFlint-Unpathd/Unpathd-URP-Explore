using UnityEngine;
using UnityEngine.UI;

public class ImageCycler : MonoBehaviour
{
    public Button nextButton; // The "Next" button
    private int currentIndex = 0; // Current index of the visible image
    private Transform[] images; // Array to hold child images

    void Start()
    {
        // Get all child images
        images = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            images[i] = transform.GetChild(i);
            images[i].gameObject.SetActive(false); // Hide all images initially
        }

        // Show the first image if there are any images
        if (images.Length > 0)
        {
            images[currentIndex].gameObject.SetActive(true);
        }

        // Add listener to the next button
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(ShowNextImage);
        }
    }

    void ShowNextImage()
    {
        // Hide the current image
        images[currentIndex].gameObject.SetActive(false);

        // Increment the index and wrap around if necessary
        currentIndex = (currentIndex + 1) % images.Length;

        // Show the next image
        images[currentIndex].gameObject.SetActive(true);
    }
}
