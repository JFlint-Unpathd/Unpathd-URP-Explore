using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MapSpawnAndToggle : MonoBehaviour
{
    

    private List<GameObject> shippingForecastRegions = new List<GameObject>();
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>();
    
    // To be able to acess the variable from the socketinteractormanger script
    public List<GameObject> GetShippingForecastRegions() 
    {
        return shippingForecastRegions;
    }

    private bool regionsVisible = false;

    private XRBaseInteractable interactable;
    private SocketInteractorManager socketInteractorManager;

    void Awake()
    {

    }

    void Start()
    {
        foreach (Transform child in transform)
        {
            GameObject childObject = child.gameObject;

            // Store original position and rotation of each child object
            originalPositions[childObject] = childObject.transform.position;
            originalRotations[childObject] = childObject.transform.rotation;
            
            // Add child object to the list of shipping forecast regions
            shippingForecastRegions.Add(childObject);
        }

        interactable = GetComponent<XRBaseInteractable>();
        interactable.onHoverEntered.AddListener(OnHoverEnter);
        interactable.onHoverEntered.AddListener(OnSelectExited);

        // Find the SocketInteractorManager in the scene
        socketInteractorManager = FindObjectOfType<SocketInteractorManager>();
        if (socketInteractorManager == null)
        {
            Debug.LogError("SocketInteractorManager not found in the scene.");
        }

    }

    public void ResetUnsnappedObjectPositions()
    {
        foreach (var childObject in shippingForecastRegions)
        {
           
            ChildObjectController childController = childObject.GetComponent<ChildObjectController>();
            if (childController != null && !childController.isSnapped && !childController.isGrabbed)
            {
                // Restore original position and rotation
                Vector3 originalPosition = originalPositions[childObject];
                Quaternion originalRotation = originalRotations[childObject];
                childObject.transform.SetPositionAndRotation(originalPosition, originalRotation);

            }
        }
    }



    // Handle hover enter for the entire parent object
    private void OnHoverEnter(XRBaseInteractor interactor)
    {
        ResetUnsnappedObjectPositions();
        // Toggle the visibility state of all child objects
        regionsVisible = !regionsVisible;
        SetChildObjectsActive(regionsVisible);

    }
    private void OnSelectExited(XRBaseInteractor interactor)
    {
        ResetUnsnappedObjectPositions();
    }

    private void SetChildObjectsActive(bool active)
    {
        foreach (var childObject in shippingForecastRegions)
        {
            ChildObjectController childController = childObject.GetComponent<ChildObjectController>();
            if (childController != null && !childController.isSnapped && !childController.isGrabbed)
            {
                // Activate or deactivate child objects based on conditions
                childObject.SetActive(active);
            }
        }
    }

    private void ClearListsAndDictionaries()
    {
        ResetUnsnappedObjectPositions();
        shippingForecastRegions.Clear();
        

    }

}


