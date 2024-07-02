using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BirdsEyeView : MonoBehaviour
{
 
    public GameObject xrOrigin;
    private GameObject birdPlane;

    public Rigidbody xrOriginRigidbody;    
    private XRBaseInteractable interactable;

    private bool isSelected = false;
    private float originalYPosition;

    private SqliteController sqliteController;

    float yOffset = 0.2f; 

    void Awake()
    {
        
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelected);
    }

    private void Start()
    {
        sqliteController = FindObjectOfType<SqliteController>();

        if (sqliteController == null)
        {
            Debug.LogError("SqliteController not found in the scene.");
        }
        else
        {
            Debug.Log("SqliteController found.");
        }

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
        
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        Vector3 offset = new Vector3(1f, 1f, 1f);
        
        if (!isSelected) // For the first selection
        {

            //Adjust the positions of all resources
            foreach (var resource in sqliteController.GetResourceDict().Values)
            {
                if (resource != null)
                {
                    resource.transform.position = new Vector3(resource.transform.position.x, 0.5f, resource.transform.position.z);
                }
                else
                {
                    Debug.LogWarning("A resource was found to be null during position adjustment.");
                }
            }

            if (xrOrigin != null)
            {
                xrOrigin.transform.position = transform.position - offset;
            }

            // Find the birdPlane GameObject using the BirdPlane tag even if it's inactive
            birdPlane = GameObject.FindGameObjectWithTag("BirdPlane");
            if (birdPlane == null)
            {
                Debug.LogError("BirdPlane not found");
                return;
            }

            isSelected = true;
        }

        else // For the second selection
        {
            // Restore the original positions of all resources
            foreach (var entry in sqliteController.GetOriginalPositions())
            {
                if (entry.Key != null)
                {
                    entry.Key.transform.position = entry.Value;
                }
                else
                {
                    Debug.LogWarning("A resource was found to be null during position restoration.");
                }
            }

            if (xrOrigin != null)
            {
                xrOrigin.transform.position = new Vector3(xrOrigin.transform.position.x, originalYPosition, xrOrigin.transform.position.z);
            }

            isSelected = false;
        
        }
    }
}
