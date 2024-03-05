using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverFloat : MonoBehaviour
{
    private XRBaseInteractable xrInteractable;
    private Rigidbody rb;
    private Vector3 originalPosition;
    private bool isHovering = false;

    // Height for the object to float up
    public float floatHeight = 1f;

    void Start()
    {
        xrInteractable = GetComponent<XRBaseInteractable>();
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;

        xrInteractable.hoverEntered.AddListener(HoverEnter);
        xrInteractable.hoverExited.AddListener(HoverExit);
    }

    void HoverEnter(HoverEnterEventArgs arg)
    {
        isHovering = true;
        StartFloating();
    }

    void HoverExit(HoverExitEventArgs arg)
    {
        isHovering = false;
        StopFloating();
    }

    void FixedUpdate()
    {
        if (!isHovering && !xrInteractable.isSelected)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    void StartFloating()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        Vector3 targetPosition = originalPosition + Vector3.up * floatHeight;
        rb.MovePosition(targetPosition);
    }

    void StopFloating()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
    }
}
