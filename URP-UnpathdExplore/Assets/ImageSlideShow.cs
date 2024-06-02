using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSlideshow : MonoBehaviour
{
    public RectTransform contentPanel;
    public float slideSpeed = 20f;
    public float waitTime = 2f;

    private float contentWidth;
    private float containerWidth;
    private float imageWidth = 30f; // Width of a single image

    void Start()
    {
        containerWidth = contentPanel.parent.GetComponent<RectTransform>().rect.width;
        contentWidth = imageWidth * 4; // Total width of all images
        StartCoroutine(SlideImages());
    }

    IEnumerator SlideImages()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            float targetPosition = contentPanel.anchoredPosition.x - imageWidth;
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
}
