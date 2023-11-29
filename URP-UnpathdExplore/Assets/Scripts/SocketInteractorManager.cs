
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketInteractorManager : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;
    private List<GameObject> snappedObjects = new List<GameObject>();

    private void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketInteractor.onSelectEntered.AddListener(AddAndHandleSelection);
    }

    private void AddAndHandleSelection(XRBaseInteractable interactable)
    {
        GameObject snappedObject = interactable.gameObject;
        UnpathSelector unpathSelector = snappedObject.GetComponent<UnpathSelector>();

        if (unpathSelector != null)
        {
            snappedObjects.Add(snappedObject); // Add snapped object to our list
            unpathSelector.HandleSelection();  // Call the HandleSelection method
        }
        else
        {
            Debug.Log("The snapped object does not have an UnpathSelector script attached.");
        }
    }

    private void OnDestroy()
    {
        // It's a good practice to remove listeners when they're no longer needed
        socketInteractor.onSelectEntered.RemoveListener(AddAndHandleSelection);
    }
}
