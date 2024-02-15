using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BirdsEyeView : MonoBehaviour
{
    //[SerializeField]
    private GameObject xrOrigin;
    //[SerializeField]
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
        

        if (interactable == null)
        {
            Debug.LogError("XRGrabInteractable component not found on the birdsEye object. Make sure it is attached.");
            return;
        }

    
        interactable.onSelectEntered.AddListener(OnSelected);

        birdPlane = GameObject.FindWithTag("BirdPlane");
        birdPlane.SetActive(false);
    }

    private void OnSelected(XRBaseInteractor interactor)
    {
        Vector3 offset = new Vector3(1f, 3f, 1f);  // Replace with your desired offset
        
        if (!isSelected) // For the first selection
        {
            xrOrigin.transform.position = transform.position - offset;
            isSelected = true; // Switch the state to true after first selection
            birdPlane.SetActive(true);
        }
        else // For the second selection
        {
            xrOrigin.transform.position = new Vector3(xrOrigin.transform.position.x, originalYPosition, xrOrigin.transform.position.z);
            isSelected = false; // Switch the state back to false after second selection
            birdPlane.SetActive(false);
        }
    }
}
