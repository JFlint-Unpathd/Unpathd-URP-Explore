
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketInteractorManager : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;
    private List<GameObject> snappedObjects = new List<GameObject>();

    // A static variable to keep track of the currently snapped object
    public static GameObject CurrentSnappedObject;
    public static GameObject CurrentChildSnappedObject;

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

        if (unpathSelector != null)
        {
            CurrentSnappedObject = snappedObject;
            CurrentChildSnappedObject = snappedObject;

            snappedObjects.Add(snappedObject); // Add snapped object to our list
            unpathSelector.HandleSelection();  // Call the HandleSelection method
            snappedObject.transform.parent = transform; // Make the snapped object a child of the socket interactor

        }

        else if (spawnAndToggle != null)
        {
            CurrentSnappedObject = snappedObject;
            spawnAndToggle.DisableSpawnedObjects();  // Disable spawned child objects
        }

        else
        {
            Debug.Log("The snapped object does not have an UnpathSelector or SpawnAndToggle script attached.");
        }
    }

    private void RemoveAndHandleDeselection(XRBaseInteractable interactable)
    {
        //Debug.Log("RemoveAndHandleDeselection called"); // Debug statement

        GameObject unsnappedObject = interactable.gameObject;
        SpawnAndToggle spawnAndToggle = unsnappedObject.GetComponent<SpawnAndToggle>();
        
        //Debug.Log("SpawnAndToggle component: " + spawnAndToggle); // Debug statement

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
    }

    private void OnDestroy()
    {
        //remove listeners when they're no longer needed
        socketInteractor.onSelectEntered.RemoveListener(AddAndHandleSelection);
        socketInteractor.onSelectExited.RemoveListener(RemoveAndHandleDeselection);
    }
}
