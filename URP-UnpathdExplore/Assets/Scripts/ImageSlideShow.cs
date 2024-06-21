// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using UnityEngine.EventSystems;
// using UnityEngine.XR.Interaction.Toolkit;

// public class ImageSlideShow : MonoBehaviour
// {
//     public HorizontalLayoutGroup layoutGroup;
//     public float slideSpeed = 20f;
//     public float waitTime = 2f;
//     // public XRBaseInteractable interactable;

//     private RectTransform[] imageRects;
//     private int imageCount;
//     private Coroutine slideRoutine;
//     private bool isCoroutineRunning = true; // Default to true to ensure slideshow starts

//     public bool IsCoroutineRunning
//     {
//         get { return isCoroutineRunning; }
//         set { isCoroutineRunning = value; }
//     }

//     private void Start()
//     {
//         imageCount = layoutGroup.transform.childCount;
//         imageRects = new RectTransform[imageCount];

//         for (int i = 0; i < imageCount; i++)
//         {
//             imageRects[i] = layoutGroup.transform.GetChild(i).GetComponent<RectTransform>();
//         }

//         slideRoutine = StartCoroutine(SlideImages());
//         // interactable.onHoverEntered.AddListener(OnHoverEnter);
//         // interactable.onHoverExited.AddListener(OnHoverExit);
//     }

//     private IEnumerator SlideImages()
//     {
//         while (true)
//         {
//             yield return new WaitForSeconds(waitTime);

//             float targetX = -imageRects[0].rect.width; // Target position to move first image out of the canvas
//             float startX = 0f; // Start position is at the beginning

//             // Slide all images to the left
//             while (startX > targetX)
//             {
//                 for (int i = 0; i < imageCount; i++)
//                 {
//                     Vector2 currentPosition = imageRects[i].anchoredPosition;
//                     currentPosition.x -= slideSpeed * Time.deltaTime;
//                     imageRects[i].anchoredPosition = currentPosition;

//                     // Wrap around logic: Move image to the right side of the canvas once it's out of view
//                     if (imageRects[i].anchoredPosition.x <= -imageRects[i].rect.width)
//                     {
//                         float lastX = imageRects[(i + imageCount - 1) % imageCount].anchoredPosition.x;
//                         float lastWidth = imageRects[(i + imageCount - 1) % imageCount].rect.width;
//                         imageRects[i].anchoredPosition = new Vector2(lastX + lastWidth, imageRects[i].anchoredPosition.y);
//                     }
//                 }

//                 startX = imageRects[0].anchoredPosition.x;
//                 yield return null;
//             }
//         }
//     }

// }

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ImageSlideShow : MonoBehaviour
{
    public HorizontalLayoutGroup layoutGroup;
    public float slideSpeed = 20f;
    public float waitTime = 2f;

    private List<RectTransform> imageRects = new List<RectTransform>();
    private Coroutine slideRoutine;

    private void Start()
    {
        RecognizeImages();
        slideRoutine = StartCoroutine(SlideImages());
    }

    private void RecognizeImages()
    {
        int childCount = layoutGroup.transform.childCount;
        imageRects.Clear();

        for (int i = 0; i < childCount; i++)
        {
            RectTransform rectTransform = layoutGroup.transform.GetChild(i).GetComponent<RectTransform>();
            imageRects.Add(rectTransform);
        }

        // Optionally, you can sort images based on their initial position or any other criteria
        // Example: imageRects.Sort((a, b) => a.localPosition.x.CompareTo(b.localPosition.x));

        // Set initial positions based on index and width
        float totalWidth = 0f;
        for (int i = 0; i < imageRects.Count; i++)
        {
            imageRects[i].anchoredPosition = new Vector2(totalWidth, 0f);
            totalWidth += imageRects[i].rect.width;
        }
    }

    private IEnumerator SlideImages()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            float targetX = -imageRects[0].rect.width;

            while (imageRects[0].anchoredPosition.x > targetX)
            {
                for (int i = 0; i < imageRects.Count; i++)
                {
                    Vector2 currentPosition = imageRects[i].anchoredPosition;
                    currentPosition.x -= slideSpeed * Time.deltaTime;
                    imageRects[i].anchoredPosition = currentPosition;

                    // Wrap around logic
                    if (imageRects[i].anchoredPosition.x <= -imageRects[i].rect.width)
                    {
                        int prevIndex = (i + imageRects.Count - 1) % imageRects.Count;
                        float lastX = imageRects[prevIndex].anchoredPosition.x;
                        float lastWidth = imageRects[prevIndex].rect.width;
                        imageRects[i].anchoredPosition = new Vector2(lastX + lastWidth, imageRects[i].anchoredPosition.y);
                    }
                }

                yield return null;
            }
        }
    }
}

