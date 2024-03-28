using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChildObjectController : MonoBehaviour
{
    public bool isChild = true;
    public bool isHovered = false;
    public bool isGrabbed = false;
    public bool isReleased = true;
    public bool isSnapped = false;

    private XRGrabInteractable grabInteractable;

    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.onHoverEntered.AddListener(OnHover);
        grabInteractable.onSelectEntered.AddListener(OnGrab);
        grabInteractable.onSelectExited.AddListener(OnRelease);
    }

    private void OnHover(XRBaseInteractor interactor)
    {
        isHovered = true;
        //Debug.Log(gameObject.name + " is hovered.");
    }

    private void OnGrab(XRBaseInteractor interactor)
    {
        isGrabbed = true;
        isReleased = false;
        //Debug.Log(gameObject.name + " is grabbed.");
    }

    private void OnRelease(XRBaseInteractor interactor)
    {
        isGrabbed = false;
        isReleased = true;
        //Debug.Log(gameObject.name + " is released.");
    }

    public void OnSnapped()
    {
        isSnapped = true;
        //Debug.Log(gameObject.name + " is snapped.");
    }

    public void OnUnsnapped()
    {
        isSnapped = false;
        //Debug.Log(gameObject.name + " is unsnapped.");
    }
}
