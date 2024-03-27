using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        
        // Retrieve original position and rotation from PrefabInstantiator
        PrefabInstantiator prefabInstantiator = gameObject.GetComponent<PrefabInstantiator>();
        if (prefabInstantiator != null)
        {
            Vector3 originalPosition = prefabInstantiator.GetOriginalPosition();
            Quaternion originalRotation = prefabInstantiator.GetOriginalRotation();

            // Add debug logs
            Debug.Log("Original Position in Start: " + originalPosition);
            Debug.Log("Original Rotation in Start: " + originalRotation);
        }
        else
        {
            Debug.LogError("PrefabInstantiator not found on GameObject.");
        }

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
        yield return new WaitForSeconds(10f);
        StopFloating();
    }

    public void StartFloating()
    {
        // Add debug logs
        Debug.Log("Original Position before floating: " + originalPosition);
        Debug.Log("Original Rotation before floating: " + originalRotation);

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

            //SpawnAndToggle.ResetParentAndSpawnedObjects();
        }

        StartCoroutine(InterpolatePosition(originalPosition, returnSpeed));
    }

    IEnumerator InterpolatePosition(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.005f) // 0.05f is a small value to decide when to stop interpolation
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
