
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketInteractorManager : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;
    private List<GameObject> snappedObjects = new List<GameObject>();

    // A static variable to keep track of the currently snapped object
    public static GameObject CurrentSnappedObject;
    public static GameObject CurrentChildSnappedObject;


    public List<GameObject> GetSnappedObjects()
    {
        return snappedObjects;
    }

    private void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketInteractor.onSelectEntered.AddListener(AddAndHandleSelection);
        socketInteractor.onSelectExited.AddListener(RemoveAndHandleDeselection);
    }


    public void AddAndHandleSelection(XRBaseInteractable interactable)
    {
        GameObject snappedObject = interactable.gameObject;

        UnpathSelector unpathSelector = snappedObject.GetComponent<UnpathSelector>();
        SpawnAndToggle spawnAndToggle = snappedObject.GetComponent<SpawnAndToggle>();
        MapSpawnAndToggle mapSpawnAndToggle = snappedObject.GetComponent<MapSpawnAndToggle>();

        if (unpathSelector != null)
        {
            CurrentSnappedObject = snappedObject;
            CurrentChildSnappedObject = snappedObject;

            snappedObjects.Add(snappedObject); // Add snapped object to snappedobj list
            unpathSelector.HandleSelection();  // Call the HandleSelection method
            snappedObject.transform.parent = transform; // Make the snapped object a child of the socket interactor

        }

        else if (spawnAndToggle != null)
        {
            CurrentSnappedObject = snappedObject;
            spawnAndToggle.DisableSpawnedObjects();  // Disable spawned child objects
        }

        else if (mapSpawnAndToggle != null)
        {
            CurrentChildSnappedObject = snappedObject;
        }

        else
        {
            Debug.Log("The snapped object does not have an UnpathSelector or SpawnAndToggle script attached.");
        }
    }



    private void RemoveAndHandleDeselection(XRBaseInteractable interactable)
    {

        GameObject unsnappedObject = interactable.gameObject;
        SpawnAndToggle spawnAndToggle = unsnappedObject.GetComponent<SpawnAndToggle>();
        MapSpawnAndToggle mapSpawnAndToggle = unsnappedObject.transform.parent?.GetComponent<MapSpawnAndToggle>();
        
        // Remove from snappedObjects list when an object is unsnapped
        snappedObjects.Remove(unsnappedObject);

        if (unsnappedObject == CurrentChildSnappedObject)
        {
            CurrentChildSnappedObject = null;
            
        }
        
        else if (spawnAndToggle != null)  // Check if the unsnapped object is the main parent object
        {
            CurrentSnappedObject = null;
            Debug.Log("Enabling spawned objects"); // Debug statement
            spawnAndToggle.EnableSpawnedObjects();  // Re-enable spawned child objects
        }

        if (mapSpawnAndToggle != null)  // Handle deselection for MapSpawnAndToggle
        {
            
            CurrentSnappedObject = null;
            CurrentChildSnappedObject = null;
            mapSpawnAndToggle.ResetUnsnappedObjectPositions();
            
        }

        
        StartCoroutine(DelayedUnparent(unsnappedObject));
    }

    private IEnumerator DelayedUnparent(GameObject objToUnparent)
    {
        // Wait for a short delay before unparenting
        yield return new WaitForEndOfFrame();

        // Unparent the unsnapped object, allowing it to naturally exit the interaction state
        objToUnparent.transform.parent = null;
    }


    private void OnDestroy()
    {
        //remove listeners when they're no longer needed
        socketInteractor.onSelectEntered.RemoveListener(AddAndHandleSelection);
        socketInteractor.onSelectExited.RemoveListener(RemoveAndHandleDeselection);
    }

    public void ClearSnappedObjects()
    {
        snappedObjects.Clear();
        
    }

    public void ClearCurrentSnappedObjects()
    {
        CurrentSnappedObject = null;
        CurrentChildSnappedObject = null;
    }

    
}

