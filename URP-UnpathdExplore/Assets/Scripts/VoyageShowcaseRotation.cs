using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VoyageShowcaseRotation : MonoBehaviour
{
    private bool isRotating = true;
    private bool isGrabbed = false;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private XRGrabInteractable grabInteractable;
    public float rotationSpeed = 360f;
    public Transform anchorPoint;

    void Start()
    {
        originalRotation = transform.rotation;
        originalPosition = transform.position;

        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.hoverEntered.AddListener(OnHoverStart);
        grabInteractable.hoverExited.AddListener(OnHoverEnd);
        grabInteractable.selectEntered.AddListener(OnGrabStart);
        grabInteractable.selectExited.AddListener(OnGrabEnd);
    }

    void OnHoverStart(HoverEnterEventArgs args)
    {
        isRotating = false; // Pause rotation when hovered over
    }

    void OnHoverEnd(HoverExitEventArgs args)
    {
        isRotating = true; // Resume rotation when hover ends
    }

   void OnGrabStart(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    void OnGrabEnd(SelectExitEventArgs args)
    {
        if (!args.isCanceled)
        {
            isGrabbed = false;
            transform.position = originalPosition; 
            transform.rotation = originalRotation;
        }
    }

    void FixedUpdate()
    {
        if (!isGrabbed)
        {
            if (isRotating)
            {
                // Calculate the angle to rotate
                float angle = rotationSpeed * Time.fixedDeltaTime;
                // Rotate around the anchor point by the specified angle
                transform.RotateAround(anchorPoint.position, Vector3.up, angle);
            }
        }
    }
}
