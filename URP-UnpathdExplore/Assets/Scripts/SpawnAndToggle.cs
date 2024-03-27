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
    [SerializeField] float spawnRadius = 0.3f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    
    // To be able to acess the variable from the socketinteractormanger script
    public List<GameObject> GetSpawnedObjects() 
    {
        return spawnedObjects;
    }

    public bool isChildGrabbedOrSnapped = false;

   
 
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

        interactable.onHoverEntered.AddListener(OnHoverEnter);
        interactable.onSelectEntered.AddListener(OnSelectEnter);
        interactable.onSelectExited.AddListener(OnSelectExit);

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
            Vector3 newPosition = new Vector3(spawnX, spawnY, transform.position.z);

            //original orientation
            //float spawnZ = transform.position.z + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
            //Vector3 newPosition = new Vector3(spawnX, transform.position.y, spawnZ);

            spawnedObjects[i].transform.position = newPosition;
        }
    }


    private void OnHoverEnter(XRBaseInteractor interactor)
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


    private void OnSelectEnter(XRBaseInteractor interactor)
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

    private void OnSelectExit(XRBaseInteractor interactor)
    {
        StartCoroutine(DelayedOnSelectExit());

    }

    private IEnumerator DelayedOnSelectExit()
    {
        yield return null; // Wait for next frame

        if (parentController != null)
        {
            parentController.isGrabbed = false;

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

    private void SpawnObjects()
    {
    
        Vector3 originalObjectPosition = transform.position;

        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            // Skip this iteration if the object already exists
            if(spawnedObjects.Exists(spawnedObject => spawnedObject.name == objectsToSpawn[i].name))
            {
                continue; 
            }

            float angle = i * (360f / objectsToSpawn.Length);
            float spawnX = originalObjectPosition.x + spawnRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
            //float spawnZ = originalObjectPosition.z + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle); //original rotation
            float spawnY = originalObjectPosition.y + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);

            //Vector3 spawnPosition = new Vector3(spawnX, originalObjectPosition.y, spawnZ); //original rotation
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, originalObjectPosition.z);

            GameObject spawnedObject = Instantiate(objectsToSpawn[i], spawnPosition, Quaternion.identity);
            
            // Set the parent of the spawned object to be this script's transform
            spawnedObject.transform.parent = transform;
            spawnedObject.SetActive(true);
            spawnedObjects.Add(spawnedObject);

            // After spawning the object, lock its position and rotation
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

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


    private bool CheckIfAnyChildSnapped()
    {
        foreach (var spawnedObject in spawnedObjects)
        {
            ChildObjectController childController = spawnedObject.GetComponent<ChildObjectController>();
            if(childController != null && childController.isSnapped)
            {
                return true;
            }
        }
        return false;
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

    public void ResetParentAndSpawnedObjects()
    {
        if (prefabInstantiator != null)
        {
            Vector3 originalPosition = prefabInstantiator.GetOriginalPosition();
            Quaternion originalRotation = prefabInstantiator.GetOriginalRotation();

            // Resetting parent position, rotation
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
        else
        {
            Debug.LogError("PrefabInstantiator is null. Cannot reset to the original position and rotation.");
        }
        
        UpdateChildPositions();
    }

}
