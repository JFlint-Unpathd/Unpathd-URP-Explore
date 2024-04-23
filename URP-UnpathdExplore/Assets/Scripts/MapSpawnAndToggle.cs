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
            // GameObject childObject = child.gameObject;

            // // Store original position and rotation of each child object
            // originalPositions[childObject] = childObject.transform.position;
            // originalRotations[childObject] = childObject.transform.rotation;

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
                Vector3 originalPosition = originalPositions[childObject];
                Quaternion originalRotation = originalRotations[childObject];
                childObject.transform.SetPositionAndRotation(originalPosition, originalRotation);

            }
        }
    }



    // public void ResetUnsnappedObjectPositions()
    // {
    //     // Get the TransformKeeper script from the parent object
    //     TransformKeeper transformKeeper = GetComponent<TransformKeeper>();

    //     // Get the original position and rotation of the parent object
    //     Vector3 parentOriginalPosition = transformKeeper.GetOriginalPosition();
    //     Quaternion parentOriginalRotation = transformKeeper.GetOriginalRotation();

    //     foreach (var childObject in shippingForecastRegions)
    //     {
    //         ChildObjectController childController = childObject.GetComponent<ChildObjectController>();
    //         if (childController != null && !childController.isSnapped && !childController.isGrabbed)
    //         {
    //             // Calculate the original local position and rotation of the child relative to the original position and rotation of the parent
    //             Vector3 originalLocalPosition = parentOriginalPosition + transform.InverseTransformPoint(originalPositions[childObject]);
    //             Quaternion originalLocalRotation = parentOriginalRotation * Quaternion.Inverse(transform.rotation) * originalRotations[childObject];
                
    //             // Reset the position and rotation of the child
    //             childObject.transform.localPosition = originalLocalPosition;
    //             childObject.transform.localRotation = originalLocalRotation;
    //         }
    //     }
    // }



    // Handle hover enter for the entire parent object
    private void OnHoverEnter(XRBaseInteractor interactor)
    {
        //ResetUnsnappedObjectPositions();
        // Toggle the visibility state of all child objects
        regionsVisible = !regionsVisible;
        SetChildObjectsActive(regionsVisible);

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
        ResetUnsnappedObjectPositions();
        shippingForecastRegions.Clear();
        

    }

}


