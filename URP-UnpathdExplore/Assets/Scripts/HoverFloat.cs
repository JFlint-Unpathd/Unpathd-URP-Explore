using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverFloat : MonoBehaviour
{
    private XRBaseInteractable xrInteractable;
    private Rigidbody rb;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    
    private bool hasBeenHoveredOver = false;

    // Reference to the script holding the spawned objects
    private SpawnAndToggle SpawnAndToggle;

    
    public float floatHeight = 0.5f; 
    public float returnSpeed = 3f; 

    void Start()
    {
        xrInteractable = GetComponent<XRBaseInteractable>();
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Lock rotation along all axes
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        SpawnAndToggle = GetComponent<SpawnAndToggle>();
        if (SpawnAndToggle == null)
        {
            // Log a debug message if the script is not found
            Debug.Log("SpawnAndToggle not found on GameObject: " + gameObject.name);
        }

        // Listners 
        xrInteractable.onHoverEntered.AddListener(OnHoverEnter);
        
    }

    private void OnHoverEnter(XRBaseInteractor interactor)
    {
        if (!hasBeenHoveredOver)
        {
            hasBeenHoveredOver = true;
            StartCoroutine(HandleFloating());
        }
    }

    IEnumerator HandleFloating()
    {
        StartFloating();
        yield return new WaitForSeconds(3f);
        StopFloating();
    }

    public void StartFloating()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        transform.rotation = originalRotation;
        Vector3 targetPosition = originalPosition + Vector3.up * floatHeight;
        rb.MovePosition(targetPosition);

    }


    public void StopFloating()
    {
        // Only call ToggleSpawnedObjectsVisibility if SpawnAndToggle is not null
        if (SpawnAndToggle != null)
        {
            SpawnAndToggle.ToggleSpawnedObjectsVisibility();
        }

        StartCoroutine(InterpolatePosition(originalPosition, returnSpeed));
    }

    IEnumerator InterpolatePosition(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f) // 0.05f is a small value to decide when to stop interpolation
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; 
        }

        // When the object is close enough, make sure it's exactly at the target
        transform.position = targetPosition;

        rb.useGravity = true;
        rb.isKinematic = false;

        hasBeenHoveredOver = false; // resetting the boolean variable
    }

}
