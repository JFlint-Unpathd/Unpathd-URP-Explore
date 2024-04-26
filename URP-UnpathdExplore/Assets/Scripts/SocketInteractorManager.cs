
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketInteractorManager : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;
    private List<GameObject> snappedObjects = new List<GameObject>();
    public GameObject CurrentSnappedObject;

    public Vector3 desiredScale = new Vector3(.5f, .5f, .5f);
    
    public List<GameObject> GetSnappedObjects()
    {
        return snappedObjects;
    }

    [Obsolete]
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

        ParentObjectController parentController = snappedObject.GetComponent<ParentObjectController>();
        ChildObjectController childController = snappedObject.GetComponent<ChildObjectController>();


        if (unpathSelector != null)
        {
            CurrentSnappedObject = snappedObject;

            // Add snapped object to snappedobj list
            snappedObjects.Add(snappedObject);
            // Call the HandleSelection method
            unpathSelector.HandleSelection();  
            

        }

        if (parentController != null)
        {

            // Record the snap and send it back to the parent controller
            parentController.OnSnapped();
            
        }

        else if (childController != null)
        {
            
            // Record the snap and send it back to the child controller
            childController.OnSnapped(); 
        }

        else
        {
            Debug.Log("The snapped object does not have an UnpathSelector or an object controller script.");
        }
    }

    private void RemoveAndHandleDeselection(XRBaseInteractable interactable)
    {

        GameObject unsnappedObject = interactable.gameObject;
        ParentObjectController parentController = unsnappedObject.GetComponent<ParentObjectController>();
        ChildObjectController childController = unsnappedObject.GetComponent<ChildObjectController>();
        
        // Remove from snappedObjects list when an object is unsnapped
        snappedObjects.Remove(unsnappedObject);

        if (unsnappedObject == CurrentSnappedObject)
        {
            CurrentSnappedObject = null;
     
        }
        
        if (parentController != null)
        {
            parentController.OnUnsnapped();
        }
        else if (childController != null)
        {
            childController.OnUnsnapped();
        }
    }

    [Obsolete]
    private void OnDestroy()
    {
        //remove listeners when they're no longer needed
        socketInteractor.onSelectEntered.RemoveListener(AddAndHandleSelection);
        socketInteractor.onSelectExited.RemoveListener(RemoveAndHandleDeselection);
    }


    public void ResetSocketInteractor()
    {
        Debug.Log("Snapped object count before reset: " + snappedObjects.Count);

        foreach(GameObject go in snappedObjects)
        {
            XRBaseInteractable interactable = go.GetComponent<XRBaseInteractable>();
            if(interactable != null)
            {
                RemoveAndHandleDeselection(interactable);
            }
        }

        snappedObjects.Clear();
        ClearCurrentSnappedObjects();
        

        Debug.Log("Snapped object count after reset: " + snappedObjects.Count);
    }

      public void ClearCurrentSnappedObjects()
    {
        CurrentSnappedObject = null;
    
    }

    public void ClearSnappedObjects()
    {
        Debug.Log("ClearSnappedObjects is called");
        foreach (GameObject obj in snappedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        snappedObjects.Clear();
    }


}

