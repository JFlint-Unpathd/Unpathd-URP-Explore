using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ParentObjectController : MonoBehaviour
{
    public bool isParent = true;
    public bool isHovered = false;
    public bool isGrabbed = false;
    public bool isReleased = true;
    public bool isSnapped = false;
    
    private XRBaseInteractable grabInteractable;
    private Rigidbody rb;
    private Color originalColor;


    private void Start()
    {
        grabInteractable = GetComponent<XRBaseInteractable>();

        rb = GetComponent<Rigidbody>();

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
        //Debug.Log(gameObject.name + " is released.");
        if(isReleased)
        {
            ResetColor();
        }
        
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
  
        //Debug.Log(gameObject.name + " is snapped.");
        
    }

    public void OnUnsnapped()
    {
        isSnapped = false;
        //Debug.Log(gameObject.name + " is unsnapped.");      
        ResetScale();
   
    }

     public void SetKinematic(bool value)
    {
        rb.isKinematic = true;
    }

    public void FreezeRotation()
    {
        rb.freezeRotation = true;
    }

    public void UnfreezeRotation()
    {
        rb.freezeRotation = false;
    }


    private void ResetScale()
    {
        gameObject.transform.localScale = Vector3.one;
        //Debug.Log(gameObject.name + " scale has been reset.");
    }



}
