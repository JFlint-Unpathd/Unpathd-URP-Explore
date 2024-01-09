using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MapSpawnToggle : MonoBehaviour
{
    public GameObject parentObject;

    private List<GameObject> shippingForecastRegions = new List<GameObject>();
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();


    private bool regionsVisible = false;

    private XRBaseInteractable interactable;
    public SocketInteractorManager socketInteractorManager; 

    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();

        interactable.onHoverEntered.AddListener(OnHoverEnter);

        
        foreach (Transform child in parentObject.transform)
        {
            GameObject childObject = child.gameObject;

            // Store the original position
            originalPositions[childObject] = childObject.transform.position;

            // Add the child object to the list
            shippingForecastRegions.Add(childObject);
        }

    }

    // Handle hover enter for the entire parent object
    private void OnHoverEnter(XRBaseInteractor interactor)
    {
        // Toggle the visibility state of all child objects
        regionsVisible = !regionsVisible;
        SetChildObjectsActive(regionsVisible);
    }

   
    private void SetChildObjectsActive(bool active)
    {
        foreach (var childObject in shippingForecastRegions)
        {
            // Check if the childObject is currently snapped
            if (childObject != SocketInteractorManager.CurrentSnappedObject && childObject != SocketInteractorManager.CurrentChildSnappedObject)
            {
                // If not snapped, toggle its visibility
                if (originalPositions.ContainsKey(childObject))
                {
                    childObject.transform.position = originalPositions[childObject];
                }
                childObject.SetActive(active);
            }
        }
    }


}
