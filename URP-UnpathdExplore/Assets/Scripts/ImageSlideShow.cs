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

    private void OnEnable()
    {
        if (layoutGroup == null)
        {
            //Debug.LogError($"{gameObject.name}: HorizontalLayoutGroup is not assigned.");
            return;
        }

        RecognizeImages();

        if (imageRects.Count > 0 && slideRoutine == null)
        {
            slideRoutine = StartCoroutine(SlideImages());
        }
        else
        {
            //Debug.LogWarning($"{gameObject.name}: No images recognized in the layout group or slideshow already running.");
        }
    }

    private void OnDisable()
    {
        if (slideRoutine != null)
        {
            StopCoroutine(slideRoutine);
            slideRoutine = null;
        }
    }

    private void RecognizeImages()
    {
        //Debug.Log($"{gameObject.name}: Images being recognized");
        int childCount = layoutGroup.transform.childCount;
        imageRects.Clear();

        if (childCount == 0)
        {
            //Debug.LogWarning($"{gameObject.name}: No child objects found in the layout group.");
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            RectTransform rectTransform = layoutGroup.transform.GetChild(i).GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                imageRects.Add(rectTransform);
                //Debug.Log($"{gameObject.name}: Image {i} recognized with width: {rectTransform.rect.width}");
            }
            else
            {
                //Debug.LogWarning($"{gameObject.name}: Child {i} does not have a RectTransform component.");
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
