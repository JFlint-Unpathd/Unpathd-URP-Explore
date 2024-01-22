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
    public SocketInteractorManager socketInteractorManager;

    void Awake()
    {
        foreach (Transform child in gameObject.transform)
        {
            GameObject childObject = child.gameObject;
            originalPositions[childObject] = childObject.transform.position;
            originalRotations[childObject] = childObject.transform.rotation;
        }
    }


    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.onHoverEntered.AddListener(OnHoverEnter);
        interactable.onHoverEntered.AddListener(OnSelectExited);

        foreach (Transform child in gameObject.transform)
        {
            GameObject childObject = child.gameObject;

            // Add the child object to the list
            shippingForecastRegions.Add(childObject);
        }
    }

    public void ResetUnsnappedObjectPositions()
    {
        foreach (var childObject in shippingForecastRegions)
        {
           
            if (childObject != SocketInteractorManager.CurrentSnappedObject && childObject != SocketInteractorManager.CurrentChildSnappedObject)
            {
                Vector3 originalPosition = originalPositions[childObject];
                Quaternion originalRotation = originalRotations[childObject];
                childObject.transform.SetPositionAndRotation(originalPosition, originalRotation);
            }
        }
    }



    // Handle hover enter for the entire parent object
    private void OnHoverEnter(XRBaseInteractor interactor)
    {
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
        List<GameObject> snappedObjects = socketInteractorManager.GetSnappedObjects();
        foreach (var childObject in shippingForecastRegions)
        {
            // Check if the childObject is currently snapped
            if (!snappedObjects.Contains(childObject))
            {
                childObject.SetActive(active);
            }
        }
    }

    public void ClearListsAndDictionaries()
    {
        ResetUnsnappedObjectPositions();
        shippingForecastRegions.Clear();
        

    }

}
