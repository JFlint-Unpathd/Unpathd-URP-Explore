using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class HoverScaleLabel : MonoBehaviour
{
    public XRBaseInteractable interactable;
    public LayoutElement layoutElement; // Make this public

    private float originalWidth, originalHeight;

    private void Awake()
    {
        // No need to dynamically find XRBaseInteractable if it's assigned in the inspector

        if (layoutElement == null)
        {
            Debug.LogError("LayoutElement not assigned in the inspector.");
            return;
        }

        // Store the original preferredWidth and preferredHeight
        originalWidth = layoutElement.preferredWidth;
        originalHeight = layoutElement.preferredHeight;

        // Add listeners for the hover events
        interactable.hoverEntered.AddListener(EnlargeLayout);
        interactable.hoverExited.AddListener(ShrinkLayout);
    }

    private void EnlargeLayout(HoverEnterEventArgs args)
    {
        // Change the preferredWidth and preferredHeight to 50 and 30 respectively
        layoutElement.preferredWidth = 50;
        layoutElement.preferredHeight = 30;
    }

    private void ShrinkLayout(HoverExitEventArgs args)
    {
        // Revert the preferredWidth and preferredHeight back to the original values
        layoutElement.preferredWidth = originalWidth;
        layoutElement.preferredHeight = originalHeight;
    }
}
