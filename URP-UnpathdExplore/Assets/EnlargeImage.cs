using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverEnlargeImage : MonoBehaviour
{
    public Image originalImage; 
    public Image enlargedImage; 

    private XRBaseInteractable interactable;
    

    void Start()
    {
        enlargedImage.gameObject.SetActive(false);
        interactable = GetComponent<XRBaseInteractable>();

        // Ensure the original image is active and the enlarged image is inactive at start
        originalImage.gameObject.SetActive(true);
        enlargedImage.gameObject.SetActive(false);

        // Add event listeners for hover events
        interactable.hoverEntered.AddListener(OnHoverEnter);
        
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
           // Check if the original image is active before toggling visibility
        if (originalImage.gameObject.activeSelf)
        {
            //originalImage.gameObject.SetActive(false);
            enlargedImage.gameObject.SetActive(true);
        }

        StartCoroutine(DelayDeactivateEnlargedImage());
    }

    private IEnumerator DelayDeactivateEnlargedImage()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Deactivate the enlarged image
        enlargedImage.gameObject.SetActive(false);
        //originalImage.gameObject.SetActive(true);
    }

    
}
