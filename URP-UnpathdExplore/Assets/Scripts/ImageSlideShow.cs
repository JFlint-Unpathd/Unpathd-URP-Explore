using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class ImageSlideShow : MonoBehaviour
{
    public HorizontalLayoutGroup layoutGroup;
    public float slideSpeed = 20f;
    public float waitTime = 2f;

    private List<RectTransform> imageRects = new List<RectTransform>();
    private Coroutine slideRoutine;
    private XRBaseInteractable interactable;
    private BoxCollider boxCollider; // To adjust the size of the collider

    private bool isPaused = false;

    private void OnEnable()
    {
        // Ensure the layout group is assigned
        if (layoutGroup == null)
        {
            Debug.LogError($"{gameObject.name}: HorizontalLayoutGroup is not assigned.");
            return;
        }

        // Adjust the BoxCollider size to match the RectTransform
        AdjustColliderSize();

        RecognizeImages();

        if (imageRects.Count > 0 && slideRoutine == null)
        {
            slideRoutine = StartCoroutine(SlideImages());
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: No images recognized in the layout group or slideshow already running.");
        }

        // Find the XRBaseInteractable in the parent
        interactable = GetComponentInParent<XRBaseInteractable>();

        // Subscribe to hover events
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: No XRBaseInteractable found in parent.");
        }
    }

    private void OnDisable()
    {
        if (slideRoutine != null)
        {
            StopCoroutine(slideRoutine);
            slideRoutine = null;
        }

        // Unsubscribe from hover events
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
        }
    }

    private void AdjustColliderSize()
    {
        // Get the RectTransform and BoxCollider components
        RectTransform rectTransform = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider>();

        // Match the BoxCollider size to the RectTransform size
        if (rectTransform != null && boxCollider != null)
        {
            boxCollider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.1f);
            boxCollider.center = new Vector3(rectTransform.rect.width / 2, -rectTransform.rect.height / 2, 0);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: RectTransform or BoxCollider component is missing.");
        }
    }

    private void RecognizeImages()
    {
        int childCount = layoutGroup.transform.childCount;
        imageRects.Clear();

        if (childCount == 0)
        {
            Debug.LogWarning($"{gameObject.name}: No child objects found in the layout group.");
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            RectTransform rectTransform = layoutGroup.transform.GetChild(i).GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                imageRects.Add(rectTransform);
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: Child {i} does not have a RectTransform component.");
            }
        }

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
            if (isPaused)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(waitTime);

            float targetX = -imageRects[0].rect.width;

            while (imageRects[0].anchoredPosition.x > targetX)
            {
                if (isPaused)
                {
                    yield return null;
                    continue;
                }

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

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        isPaused = true;
        
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        isPaused = false;
       
    }
}
