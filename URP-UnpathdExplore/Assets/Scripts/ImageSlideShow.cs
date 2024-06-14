using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class ImageSlideShow : MonoBehaviour
{

    public int currentImageIndex = 0;

    public RectTransform contentPanel;
    private Coroutine slideRoutine;
    public XRBaseInteractable interactable;
    public float slideSpeed = 20f;
    public float waitTime = 2f;
    public float imageNumber = 4;

    private float contentWidth;
    private float containerWidth;
    //private float imageWidth = 30f; // Width of a single image
    private float[] imageWidths;

    void Start()
    {
        containerWidth = contentPanel.parent.GetComponent<RectTransform>().rect.width;

        imageWidths = new float[contentPanel.childCount];
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            imageWidths[i] = contentPanel.GetChild(i).GetComponent<RectTransform>().rect.width;
        }


        //contentWidth = imageWidth * imageNumber; // Total width of all images

        contentWidth = 0;
        for (int i = 0; i < imageWidths.Length; i++)
        {
            contentWidth += imageWidths[i];
        }

        slideRoutine = StartCoroutine(SlideImages());

        interactable.onHoverEntered.AddListener(OnHoverEnter);
        interactable.onHoverExited.AddListener(OnHoverExit);
    }

    IEnumerator SlideImages()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            //float targetPosition = contentPanel.anchoredPosition.x - imageWidth;
            float targetPosition = -imageWidths[currentImageIndex];
            float startPosition = contentPanel.anchoredPosition.x;

            while (contentPanel.anchoredPosition.x > targetPosition)
            {
                contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x - slideSpeed * Time.deltaTime, contentPanel.anchoredPosition.y);

                // Ensure we don't move past the target position
                if (contentPanel.anchoredPosition.x <= targetPosition)
                {
                    contentPanel.anchoredPosition = new Vector2(targetPosition, contentPanel.anchoredPosition.y);
                    break;
                }

                yield return null;
            }

            // Reset position for looping if at the end
            if (Mathf.Abs(contentPanel.anchoredPosition.x) >= contentWidth - containerWidth)
            {
                contentPanel.anchoredPosition = new Vector2(0, contentPanel.anchoredPosition.y);
            }
        }
    }

    public void OnHoverEnter(XRBaseInteractor interactor)
    {
        // The controller is hovering over the object, so stop the slideshow
        StopCoroutine(slideRoutine);
    }

    public void OnHoverExit(XRBaseInteractor interactor)
    {
        // The controller has stopped hovering over the object, so resume the slideshow
        slideRoutine = StartCoroutine(SlideImages());
    }


}