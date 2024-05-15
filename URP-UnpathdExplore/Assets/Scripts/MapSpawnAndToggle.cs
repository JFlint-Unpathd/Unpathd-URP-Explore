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

            // Store original local position and local rotation of each child object
            originalPositions[childObject] = childObject.transform.localPosition;
            originalRotations[childObject] = childObject.transform.localRotation;
            
            // Add child object to the list of shipping forecast regions
            shippingForecastRegions.Add(childObject);

            // Access ChildObjectController script and set the object to kinematic
            ChildObjectController childController = childObject.GetComponent<ChildObjectController>();
            if (childController != null)
            {
                childController.KinematicChild(true);
            }

            // Make the child object inactive at the start
            childObject.SetActive(false);
        }

        interactable = GetComponent<XRBaseInteractable>();
        interactable.onHoverEntered.AddListener(OnHoverEnter);
        interactable.onHoverEntered.AddListener(OnHoverExited);

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
                Vector3 originalLocalPosition = originalPositions[childObject];
                Quaternion originalLocalRotation = originalRotations[childObject];
                childObject.transform.localPosition = originalLocalPosition;
                childObject.transform.localRotation = originalLocalRotation;
            }
        }
    }



    // Handle hover enter for the entire parent object
    private void OnHoverEnter(XRBaseInteractor interactor)
    {
        //ResetUnsnappedObjectPositions();
        // Toggle the visibility state of all child objects
        // these are the controls for toggling children on/ off, rather than just on
        //regionsVisible = !regionsVisible;
        //SetChildObjectsActive(regionsVisible);

        if (!regionsVisible)
        {
            SetChildObjectsActive(true);
            regionsVisible = true;
        }

    }

    
    private void OnHoverExited(XRBaseInteractor interactor)
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
        //ResetUnsnappedObjectPositions();
        shippingForecastRegions.Clear();
        

    }
    

}


