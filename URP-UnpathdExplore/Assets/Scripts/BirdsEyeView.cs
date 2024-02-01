using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BirdsEyeView : MonoBehaviour
{
    //[SerializeField]
    private GameObject xrOrigin;
    //[SerializeField]
    private Rigidbody xrOriginRigidbody;    

    private XRGrabInteractable grabInteractable;

    private bool isSelected = false;
    private float originalYPosition;


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

        // Automatically assign XRGrabInteractable on the same GameObject
        grabInteractable = GetComponent<XRGrabInteractable>();
        xrOriginRigidbody.isKinematic = true;
        
        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable component not found on the birdsEye object. Make sure it is attached.");
            return;
        }

        // Subscribe to the selection event
        grabInteractable.onSelectEntered.AddListener(OnSelected);
    }

    private void OnSelected(XRBaseInteractor interactor)
    {
        
        if (!isSelected) // For the first selection
        {
            xrOrigin.transform.position = transform.position;
            isSelected = true; // Switch the state to true after first selection
        }
        else // For the second selection
        {
            xrOrigin.transform.position = new Vector3(xrOrigin.transform.position.x, originalYPosition, xrOrigin.transform.position.z);
            isSelected = false; // Switch the state back to false after second selection
        }
    }
}
