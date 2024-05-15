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
    private Rigidbody rb;
    private Color originalColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            originalColor = renderer.material.color;
        }
        else
        {
            Debug.LogError("No Renderer on the object or its first child.");
        }

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

        if (!isSnapped)
        {
            // Get the TransformKeeper script attached to the object
            TransformKeeper transformKeeper = GetComponent<TransformKeeper>();

            if (transformKeeper != null)
            {
                // Reset the transform of the object
                transformKeeper.ResetAllTransform();
            }
            else
            {
                Debug.LogError("TransformKeeper script not found on object: " + gameObject.name);
            }
        }

        if(isReleased)
        {
            ResetColor();
        }

        //Debug.Log(gameObject.name + " is released.");
    }

    private void ResetColor()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = originalColor;  // set the color
        }
        else
        {
            Debug.LogError("No Renderer found so no color reset.");
        }
        
    }


    public void OnSnapped()
    {
        isSnapped = true;
        Debug.Log(gameObject.name + " is snapped.");   
        
    }

    public void OnUnsnapped()
    {
        isSnapped = false;
        //Debug.Log(gameObject.name + " is unsnapped.");
        ResetScale();

    }

      private void ResetScale()
    {
        gameObject.transform.localScale = Vector3.one;
       // Debug.Log(gameObject.name + " scale has been reset.");
    }


    public void KinematicChild(bool value)
    {
        if(rb != null)
        {
            rb.isKinematic = value;
        }
        else
        {
            Debug.LogError("Rigidbody is null.");
        }
    }



}
