using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnAndToggle : MonoBehaviour
{
    private TransformKeeper transformKeeper;
    private ParentObjectController parentController;
    private XRBaseInteractable interactable;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [SerializeField] GameObject[] objectsToSpawn;
    //[SerializeField] float spawnRadius = 1f;
    float spawnRadius = 1f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
     private Dictionary<Transform, bool> parentSpawnStates = new Dictionary<Transform, bool>();
    
    // To be able to acess the variable from the socketinteractormanger script
    public List<GameObject> GetSpawnedObjects() 
    {
        return spawnedObjects;
    }


    private void Start()
    {
        parentController = GetComponent<ParentObjectController>();

        transformKeeper = GetComponent<TransformKeeper>();
        if (transformKeeper != null)
        {
            Vector3 originalPosition = transformKeeper.GetOriginalPosition();
            Quaternion originalRotation = transformKeeper.GetOriginalRotation();
        }
        else
        {
            Debug.LogError("TransformKeeper not found on GameObject.");
        }

        
        interactable = GetComponent<XRBaseInteractable>();

        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.selectEntered.AddListener(OnSelectEnter);
        interactable.selectExited.AddListener(OnSelectExit);

    }


    private void Update()
    {
        if (parentController != null && (parentController.isGrabbed || parentController.isSnapped))
        {
            foreach (var spawnedObject in spawnedObjects)
            {
                ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
                if(childController != null && !childController.isSnapped)
                {
                    spawnedObject.SetActive(false);
                }
            }
        }

        else
        {
            UpdateChildPositions();
        }
    }


    private void UpdateChildPositions()
    {
        // Get the current parent position and rotation
        Vector3 parentPosition = transform.position;
        Quaternion parentRotation = transform.rotation;
        Vector3 parentScale = transform.lossyScale;

        int numberOfObjects = spawnedObjects.Count;
        float angleIncrement = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * angleIncrement * Mathf.Deg2Rad;

            // Calculate the new local position relative to the parent's local coordinates
            float spawnX = spawnRadius * Mathf.Cos(angle);
            float spawnY = spawnRadius * Mathf.Sin(angle);
            //Vector3 localSpawnPosition = new Vector3(spawnX, 0f, spawnY); // Assuming XY plane as desired
            Vector3 localSpawnPosition = new Vector3(spawnX, spawnY, 0f);

            // Transform the local position to world space based on the parent's position and rotation
            Vector3 worldSpawnPosition = parentPosition + parentRotation * localSpawnPosition;

            // Set the position of the spawned object to the calculated world position
            spawnedObjects[i].transform.position = worldSpawnPosition;

            // Calculate rotation to face along the parent's local up direction (e.g., XY plane)
            Quaternion spawnRotation = Quaternion.LookRotation(parentRotation * Vector3.forward, parentRotation * Vector3.up);

            // Set the rotation of the spawned object to face towards the parent's local up direction
            spawnedObjects[i].transform.rotation = spawnRotation;


        }

    }


    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        Transform parentTransform = transform;

        // Check if the parent transform is already in the dictionary
        if (!parentSpawnStates.ContainsKey(parentTransform) || !parentSpawnStates[parentTransform])
        {
            // Spawn objects for this parent if not already spawned
            SpawnObjects(parentTransform);
            parentSpawnStates[parentTransform] = true; // Update spawn state to true
        }
     
        else if (parentController != null && !parentController.isGrabbed && !parentController.isSnapped)
        {
            // Reset rotations
            foreach (var spawnedObject in spawnedObjects)
            {
                ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
                if (childController != null && !childController.isSnapped && !childController.isGrabbed)
                {
                    spawnedObject.transform.rotation = Quaternion.identity;
                    spawnedObject.transform.localScale = transform.lossyScale;
                }
            }


            ToggleSpawnedObjectsVisibility();
        }
    }


    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (parentController != null)
        {
            parentController.isGrabbed = true;

            // Hide nonsnapped spawned objects
            foreach (var spawnedObject in spawnedObjects)
            {
                ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
                if(childController != null && !childController.isSnapped)
                {
                    spawnedObject.transform.parent = null;
                    spawnedObject.SetActive(false);
                }
            }
        }
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        StartCoroutine(DelayedOnSelectExit());

    }

    private IEnumerator DelayedOnSelectExit()
    {
        yield return null; // Wait for next frame

        if (parentController != null)
        {
            parentController.isGrabbed = false;

            // Determine the desired visibility state for unsnapped objects
            bool unsnappedVisibility = GetUnsnappedVisibility();

            // Set visibility for unsnapped objects
            SetUnsnappedVisibility(unsnappedVisibility);

            //added a 2 second delay so hopefully children do not snap to socket
            yield return new WaitForSeconds(3);

            // Clear nonsnapped spawned objects
            foreach (var spawnedObject in spawnedObjects)
            {
                ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
                if(childController != null && !childController.isSnapped)
                {
                    //reparent objects
                    spawnedObject.transform.parent = transform;
                    //Destroy(spawnedObject);
                }
            }

            // Clear the list of spawned objects
            //spawnedObjects.Clear();
        }
    }

     private bool GetUnsnappedVisibility()
    {
       // Get the visibility state of the first unsnapped child
        foreach (var spawnedObject in spawnedObjects)
        {
            ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
            if (childController != null && !childController.isSnapped)
            {
                return spawnedObject.activeSelf;
            }
        }

        // If no unsnapped child is found, return true by default
        return true;

    }

        private void SetUnsnappedVisibility(bool visibility)
    {
        // Set the visibility state for all unsnapped children
        foreach (var spawnedObject in spawnedObjects)
        {
            ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
            if (childController != null && !childController.isSnapped)
            {
                spawnedObject.SetActive(visibility);
            }
        }
    }

    private void SpawnObjects(Transform parentTransform)
    {
        Debug.Log("Spawning Child spawms");

        int numberOfObjects = objectsToSpawn.Length;
        float angleIncrement = 360f / numberOfObjects;

        Vector3 parentPosition = transformKeeper.OriginalPosition; 
        Quaternion parentRotation = transformKeeper.OriginalRotation; 

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * angleIncrement * Mathf.Deg2Rad;

            float spawnX = spawnRadius * Mathf.Cos(angle);
            float spawnY = spawnRadius * Mathf.Sin(angle);
            Vector3 localPosition = new Vector3(spawnX, spawnY, 0f);

            Vector3 newPosition = parentPosition + parentRotation * localPosition;

            Quaternion newRotation = parentRotation; // Use parent's original rotation

          
            GameObject spawnedObject = Instantiate(objectsToSpawn[i], newPosition, newRotation, transform);
            spawnedObject.SetActive(true);
            spawnedObjects.Add(spawnedObject);
        
        }
    }

    public void ToggleSpawnedObjectsVisibility()
    {
        foreach (var spawnedObject in spawnedObjects)
        {
            ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
            if(childController != null)
            {
                // If the child is not snapped, toggle its visibility
                if(!childController.isSnapped)
                {
                    spawnedObject.SetActive(!spawnedObject.activeSelf);
                }
            }
            else
            {
                Debug.LogWarning("ChildObjectController not found on spawned object.");
            }
        }
    }


    public void EnableSpawnedObjects()
    {
        Debug.Log("EnableSpawnedObjects called"); // Debug statement
        foreach (var spawnedObject in spawnedObjects)
        {
            spawnedObject.SetActive(true);
        }
    }

    public void DisableSpawnedObjects()
    {
        foreach (var spawnedObject in spawnedObjects)
        {
            spawnedObject.SetActive(false);
        }
    }

    // public void ResetParentAndSpawnedObjects()
    // {
    //     if (transformKeeper != null)
    //     {
    //         Vector3 originalPosition = transformKeeper.GetOriginalPosition();
    //         Quaternion originalRotation = transformKeeper.GetOriginalRotation();

    //         // Resetting parent position, rotation
    //         transform.position = originalPosition;
    //         transform.rotation = originalRotation;
    //     }
    //     else
    //     {
    //         Debug.LogError("TransformKeeper is null. Cannot reset to the original position and rotation.");
    //     }
        
    //     UpdateChildPositions();
    // }

}
