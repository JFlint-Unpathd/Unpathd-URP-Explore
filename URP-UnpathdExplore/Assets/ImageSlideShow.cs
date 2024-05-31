using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSlideshow : MonoBehaviour
{
    public RectTransform contentPanel;
    public float slideSpeed = 50f;
    public float waitTime = 2f;

    void Start()
    {
        StartCoroutine(SlideImages());
    }

    IEnumerator SlideImages()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            float targetPosition = contentPanel.anchoredPosition.x - contentPanel.rect.width;
            while (contentPanel.anchoredPosition.x > targetPosition)
            {
                contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x - slideSpeed * Time.deltaTime, contentPanel.anchoredPosition.y);
                yield return null;
            }

            // Reset position for looping
            contentPanel.anchoredPosition = new Vector2(0, contentPanel.anchoredPosition.y);
        }
    }
}
