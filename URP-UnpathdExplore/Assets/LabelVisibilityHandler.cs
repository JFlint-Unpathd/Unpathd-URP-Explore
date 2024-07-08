using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LabelVisibilityHandler : MonoBehaviour
{
    public GameObject label;

    private XRBaseInteractable interactable;

    void Start()
    {
        // Ensure the label is initially inactive
        if (label != null)
        {
            label.SetActive(false);
        }

        interactable = GetComponent<XRBaseInteractable>();

        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
        }
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (label != null && !interactable.isSelected)
        {
            label.SetActive(true);
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (label != null)
        {
            label.SetActive(false);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (label != null)
        {
            label.SetActive(false);
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (label != null)
        {
            label.SetActive(false);
        }
    }
}
