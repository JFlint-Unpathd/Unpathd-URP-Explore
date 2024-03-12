using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverFloat : MonoBehaviour
{
    private XRBaseInteractable xrInteractable;
    private Rigidbody rb;
    private Vector3 originalPosition;

    // Height for the object to float up
    public float floatHeight = 1f;

    void Start()
    {
        xrInteractable = GetComponent<XRBaseInteractable>();
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;

        // Lock rotation along all axes
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Add listener for XRBaseInteractable events
        xrInteractable.onHoverEntered.AddListener(OnHoverEnter);
    }

    private void OnHoverEnter(XRBaseInteractor interactor)
    {
        StartCoroutine(HandleFloating());
    }

    IEnumerator HandleFloating()
    {
        // Start floating
        StartFloating();

        // Wait for 3 seconds
        yield return new WaitForSeconds(1f);

        // Then return to original position
        StopFloating();
    }

    public void StartFloating()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        Vector3 targetPosition = originalPosition + Vector3.up * floatHeight;
        rb.MovePosition(targetPosition);
    }

    // public void StopFloating()
    // {
    //     rb.useGravity = true;
    //     rb.isKinematic = false;
    //     rb.MovePosition(originalPosition);
    // }

  

    public float returnSpeed = 0.1f; // Speed at which the object returns to its original position

    public void StopFloating()
    {
        StartCoroutine(InterpolatePosition(originalPosition));
    }

    IEnumerator InterpolatePosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f) // 0.05f is a small value to decide when to stop interpolation
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, returnSpeed);
            yield return null; // Wait until next frame
        }

        // When the object is close enough, make sure it's exactly at the target
        transform.position = targetPosition;

        rb.useGravity = true;
        rb.isKinematic = false;
    }

}
