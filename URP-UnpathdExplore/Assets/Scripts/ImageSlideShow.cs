using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class ImageSlideShow : MonoBehaviour
{
    public HorizontalLayoutGroup layoutGroup;
    public float slideSpeed = 20f;
    public float waitTime = 2f;
    // public XRBaseInteractable interactable;

    private RectTransform[] imageRects;
    private int imageCount;
    private Coroutine slideRoutine;
    private bool isCoroutineRunning = true; // Default to true to ensure slideshow starts

    public bool IsCoroutineRunning
    {
        get { return isCoroutineRunning; }
        set { isCoroutineRunning = value; }
    }

    private void Start()
    {
        imageCount = layoutGroup.transform.childCount;
        imageRects = new RectTransform[imageCount];

        for (int i = 0; i < imageCount; i++)
        {
            imageRects[i] = layoutGroup.transform.GetChild(i).GetComponent<RectTransform>();
        }

        slideRoutine = StartCoroutine(SlideImages());
        // interactable.onHoverEntered.AddListener(OnHoverEnter);
        // interactable.onHoverExited.AddListener(OnHoverExit);
    }

    private IEnumerator SlideImages()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            float targetX = -imageRects[0].rect.width; // Target position to move first image out of the canvas
            float startX = 0f; // Start position is at the beginning

            // Slide all images to the left
            while (startX > targetX)
            {
                for (int i = 0; i < imageCount; i++)
                {
                    Vector2 currentPosition = imageRects[i].anchoredPosition;
                    currentPosition.x -= slideSpeed * Time.deltaTime;
                    imageRects[i].anchoredPosition = currentPosition;

                    // Wrap around logic: Move image to the right side of the canvas once it's out of view
                    if (imageRects[i].anchoredPosition.x <= -imageRects[i].rect.width)
                    {
                        float lastX = imageRects[(i + imageCount - 1) % imageCount].anchoredPosition.x;
                        float lastWidth = imageRects[(i + imageCount - 1) % imageCount].rect.width;
                        imageRects[i].anchoredPosition = new Vector2(lastX + lastWidth, imageRects[i].anchoredPosition.y);
                    }
                }

                startX = imageRects[0].anchoredPosition.x;
                yield return null;
            }
        }
    }

    // private void OnHoverEnter(XRBaseInteractor interactor)
    // {
    //     StopCoroutine(slideRoutine);
    //     isCoroutineRunning = false;
    // }

    // private void OnHoverExit(XRBaseInteractor interactor)
    // {
    //     if (!isCoroutineRunning)
    //     {
    //         slideRoutine = StartCoroutine(SlideImages());
    //         isCoroutineRunning = true;
    //     }
    // }
}
