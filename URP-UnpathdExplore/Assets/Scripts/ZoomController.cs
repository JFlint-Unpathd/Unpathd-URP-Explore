using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class ZoomController : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private bool isZoomed = false;

    // Reference to the SqliteController script
    public SqliteController sqliteController;

    // List to store selected UnpathResource objects for zoom logic
    private List<UnpathResource> selectionList = new List<UnpathResource>();

    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to the grip events
        grabInteractable.onActivate.AddListener(OnGripActivate);
        grabInteractable.onDeactivate.AddListener(OnGripDeactivate);
    }

    private void OnGripActivate(XRBaseInteractor interactor)
    {
        if (!isZoomed)
        {
            // Activate the zoom functionality
            isZoomed = true;

            // Raycast to select objects
            RaycastToSelectObjects(interactor);

            // Set all objects to invisible initially
            SetAllObjectsVisibility(false);

            // Set the selected objects to visible
            SetSelectedObjectsVisibility(selectionList, true);
        }
        else
        {
            // Deactivate the zoom functionality
            isZoomed = false;

            // Set all objects to visible
            SetAllObjectsVisibility(true);

            // Deselect all resources
            DeselectAllResources();
        }
    }

    private void OnGripDeactivate(XRBaseInteractor interactor)
    {
        // No action needed here, as grip deactivation is handled in OnGripActivate
    }

    private void RaycastToSelectObjects(XRBaseInteractor interactor)
    {
        // Perform raycasting to select objects
        RaycastHit[] hits;
        hits = Physics.RaycastAll(interactor.transform.position, interactor.transform.forward);

        // Clear the selection list
        selectionList.Clear();

        // Store selected objects in the selection list
        foreach (var hit in hits)
        {
            UnpathResource selectedResource = hit.collider.GetComponent<UnpathResource>();
            if (selectedResource != null)
            {
                selectionList.Add(selectedResource);
            }
        }
    }

    private void SetAllObjectsVisibility(bool isVisible)
    {
        // Find all objects of type UnpathResource in the scene
        UnpathResource[] allObjects = GameObject.FindObjectsOfType<UnpathResource>();

        // Set the visibility of all objects
        foreach (var obj in allObjects)
        {
            obj.gameObject.SetActive(isVisible);
        }
    }

    private void SetSelectedObjectsVisibility(List<UnpathResource> selectedObjects, bool isVisible)
    {
        // Set the visibility of selected objects
        foreach (var obj in selectedObjects)
        {
            obj.gameObject.SetActive(isVisible);
        }
    }

    // added by M for zoom logic
    private void DeselectAllResources()
    {
        foreach (var selectedResource in selectionList)
        {
            selectedResource.isSelected = false;
        }

        selectionList.Clear();
    }
}
