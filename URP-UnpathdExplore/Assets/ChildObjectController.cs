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

        grabInteractable.hoverEntered.AddListener(OnHover);
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnHover(HoverEnterEventArgs args)
    {
        isHovered = true;
        //Debug.Log(gameObject.name + " is hovered.");
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        isReleased = false;
        //Debug.Log(gameObject.name + " is grabbed.");
    }

    private void OnRelease(SelectExitEventArgs args)
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
