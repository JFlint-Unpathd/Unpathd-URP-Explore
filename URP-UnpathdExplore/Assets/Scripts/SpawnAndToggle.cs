using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnAndToggle : MonoBehaviour
{
    private PrefabInstantiator prefabInstantiator;
    private ParentObjectController parentController;
    private XRBaseInteractable interactable;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [SerializeField] GameObject[] objectsToSpawn;
    //[SerializeField] float spawnRadius = 1f;
    float spawnRadius = 1f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    
    // To be able to acess the variable from the socketinteractormanger script
    public List<GameObject> GetSpawnedObjects() 
    {
        return spawnedObjects;
    }


    private void Start()
    {
        parentController = GetComponent<ParentObjectController>();

        prefabInstantiator = GetComponent<PrefabInstantiator>();
        if (prefabInstantiator != null)
        {
            Vector3 originalPosition = prefabInstantiator.GetOriginalPosition();
            Quaternion originalRotation = prefabInstantiator.GetOriginalRotation();
        }
        else
        {
            Debug.LogError("PrefabInstantiator not found on GameObject.");
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
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            float angle = i * (360f / objectsToSpawn.Length);
            float spawnX = transform.position.x + spawnRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float spawnY = transform.position.y + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
            //float spawnZ = transform.position.z + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);

            
            //Vector3 newPosition = new Vector3(transform.position.x, spawnY, spawnZ); // Keep the X position of the parent
            //Vector3 newPosition = new Vector3(spawnX, transform.position.y, spawnZ); // Keep the Y position of the parent
            Vector3 newPosition = new Vector3(spawnX, spawnY, transform.position.z); // Keep the Z position of the parent

            spawnedObjects[i].transform.position = newPosition;
            spawnedObjects[i].transform.rotation = transform.rotation;

        }
    }


    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (spawnedObjects.Count == 0)
        {
            SpawnObjects();
        }

        else if (parentController != null && !parentController.isGrabbed && !parentController.isSnapped)
        {
            // Reset rotations
            foreach (var spawnedObject in spawnedObjects)
            {
                ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
                if(childController != null && !childController.isSnapped)
                {
                    spawnedObject.transform.rotation = Quaternion.identity;
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
            yield return new WaitForSeconds(2);

            // Clear nonsnapped spawned objects
            foreach (var spawnedObject in spawnedObjects)
            {
                ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
                if(childController != null && !childController.isSnapped)
                {
                    //reparent objects
                    spawnedObject.transform.parent = transform;
                    Destroy(spawnedObject);
                }
            }

            // Clear the list of spawned objects
            spawnedObjects.Clear();
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

 

    private void SpawnObjects()
    {
        
         Vector3 originalObjectPosition = transform.position;
        
        // Create a HashSet to store the spawned objects
        HashSet<GameObject> spawnedPrefabSet = new HashSet<GameObject>(spawnedObjects);

        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            if (spawnedPrefabSet.Contains(objectsToSpawn[i]))
            {
                continue; // Skip spawning this object if it's already spawned
            }

            float angle = i * (360f / objectsToSpawn.Length);
            float spawnX = originalObjectPosition.x + spawnRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float spawnY = originalObjectPosition.y + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
            //float spawnZ = originalObjectPosition.z + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);

            //float spawnX = originalObjectPosition.x; // Keep the 'x' value the same as the parent's
            //float spawnY = originalObjectPosition.y; // Keep the 'y' value the same as the parent's
            float spawnZ = originalObjectPosition.z; // Keep the 'z' value the same as the parent's

            Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

            
            // Calculate rotation to face the center of the parent object
            Quaternion spawnRotation = Quaternion.LookRotation(originalObjectPosition - spawnPosition);

            GameObject spawnedObject = Instantiate(objectsToSpawn[i], spawnPosition, Quaternion.identity);

            // Set the parent of the spawned object to be this script's transform
            spawnedObject.transform.parent = transform;
            spawnedObject.SetActive(true);
            spawnedObjects.Add(spawnedObject);

            // After spawning the object, lock its position and rotation
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            // Add the prefab of the spawned object to the HashSet
            spawnedPrefabSet.Add(objectsToSpawn[i]);
         
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
    //     if (prefabInstantiator != null)
    //     {
    //         Vector3 originalPosition = prefabInstantiator.GetOriginalPosition();
    //         Quaternion originalRotation = prefabInstantiator.GetOriginalRotation();

    //         // Resetting parent position, rotation
    //         transform.position = originalPosition;
    //         transform.rotation = originalRotation;
    //     }
    //     else
    //     {
    //         Debug.LogError("PrefabInstantiator is null. Cannot reset to the original position and rotation.");
    //     }
        
    //     UpdateChildPositions();
    // }

}
