using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BirdsEyeView : MonoBehaviour
{
 
    private GameObject xrOrigin;
    private Rigidbody xrOriginRigidbody;    

    private XRBaseInteractable interactable;

    private bool isSelected = false;
    private float originalYPosition;

    private GameObject birdPlane;

    void Awake()
    {
        
        interactable = GetComponent<XRBaseInteractable>();
    }

    private void Start()
    {
        
        // Find the XR Origin and its Rigidbody using the XRRig tag
        xrOrigin = GameObject.FindWithTag("XRRig");

        if (xrOrigin != null)
        {
            xrOriginRigidbody = xrOrigin.GetComponent<Rigidbody>();
            // Initialize originalYPosition after xrOrigin is assigned
            originalYPosition = xrOrigin.transform.position.y;

        }
        else
        {
            Debug.LogError("XR Origin not found with tag 'XRRig'. Make sure the XR Rig object is correctly tagged.");
            return;
        }

        if (xrOriginRigidbody == null)
        {
            Debug.LogError("Rigidbody component not found on the XR Origin. Make sure it is correctly set up.");
            return;
        }


        xrOriginRigidbody.isKinematic = true;
        
        interactable.selectEntered.AddListener(OnSelected);

    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        Vector3 offset = new Vector3(1f, 1f, 1f);  // Replace with your desired offset
        
        if (!isSelected) // For the first selection
        {
            xrOrigin.transform.position = transform.position - offset;
            isSelected = true; // Switch the state to true after first selection
            
            // Find the birdPlane GameObject using the BirdPlane tag even if it's inactive
            GameObject[] birdPlanes = GameObject.FindGameObjectsWithTag("BirdPlane");
            if (birdPlanes.Length > 0)
            {
                birdPlane = birdPlanes[0];
            }
            else
            {
                Debug.LogError("BirdPlane not found with tag 'BirdPlane'. Make sure it exists in the scene.");
                return;
            }

            // Ensure birdPlane is not null before accessing it
            if (birdPlane != null)
            {
                birdPlane.SetActive(true);
            }
            else
            {
                Debug.LogError("BirdPlane GameObject is null. Make sure it's properly initialized.");
            }
        }

        else // For the second selection
        {
            xrOrigin.transform.position = new Vector3(xrOrigin.transform.position.x, originalYPosition, xrOrigin.transform.position.z);
            isSelected = false; // Switch the state back to false after second selection
            
            // Ensure birdPlane is not null before accessing it
            if (birdPlane != null)
            {
                birdPlane.SetActive(false);
            }
            else
            {
                Debug.LogError("BirdPlane GameObject is null. Make sure it's properly initialized.");
            }
        }
    }
}
