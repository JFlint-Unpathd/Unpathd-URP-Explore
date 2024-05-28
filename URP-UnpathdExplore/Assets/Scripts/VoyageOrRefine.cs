using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class VoyageOrRefine : MonoBehaviour
{
    private string tag1 = "Voyages";
    public float spawnRadius = 1;
    public float distanceBetweenObjects = 1.0f;

    [SerializeField] GameObject[] voyageOptions;
    [SerializeField] private GameObject centerObject;
    public Transform anchorPoint; 
    private XRBaseInteractable interactable;

    private Vector3 parentPosition;
    private Quaternion parentRotation;

    private bool hasSpawned = false;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void Start()
    {
        parentPosition = transform.position;
        parentRotation = transform.rotation;
    }


    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        GameObject item = args.interactable.gameObject;
        
        if (item.tag == tag1 && !hasSpawned)
        {
            // Activate the four other voyage options
            SpawnVoyageOptionsInLine();
            hasSpawned = true; // Set to true to prevent spawning again
        }
    }

    private void SpawnVoyageOptions()
    {
        int numberOfObjects = voyageOptions.Length;
        float angleIncrement = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * angleIncrement * Mathf.Deg2Rad;

            float spawnX = anchorPoint.position.x + spawnRadius * Mathf.Cos(angle);
            float spawnY = anchorPoint.position.y + spawnRadius * Mathf.Sin(angle);
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, anchorPoint.position.z);

            Quaternion newRotation = Quaternion.identity; // No rotation

            GameObject spawnedObject = Instantiate(voyageOptions[i], spawnPosition, newRotation);
            spawnedObject.SetActive(true);
        }
    }


    private void SpawnVoyageOptionsInLine()
    {
        if (centerObject == null)
        {
            Debug.LogError("Center object is not assigned.");
            return;
        }

        // Check if the first child exists
        if (centerObject.transform.childCount == 0)
        {
            Debug.LogError("Center object has no child objects.");
            return;
        }

        GameObject firstChild = centerObject.transform.GetChild(0).gameObject;
        
        Renderer centerRenderer = firstChild.GetComponent<Renderer>();

        // Check if Renderer component is attached
        if (centerRenderer == null)
        {
            Debug.LogError("No Renderer attached to the first child of Center object");
            return;
        }

        // Set the renderer of firstChild to be invisible
        centerRenderer.enabled = false;

        // Find and disable the interactableLabel child object
        Transform interactableLabel = centerObject.transform.Find("Interactable Label");
        if (interactableLabel != null)
        {
            interactableLabel.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("interactableLabel child object not found in Center object");
        }

        float centerObjectHeight = centerRenderer.bounds.size.y;

        int numberOfObjects = voyageOptions.Length;
        int centerIndex = numberOfObjects / 2; // Center index for even distribution

        for (int i = 0; i < numberOfObjects; i++)
        {
            float offset = (i - centerIndex) * (centerObjectHeight + distanceBetweenObjects);
            Vector3 spawnPosition = anchorPoint.position + new Vector3(0, offset, 0);
            GameObject spawnedObject = Instantiate(voyageOptions[i], spawnPosition, Quaternion.identity);
            spawnedObject.SetActive(true);
            spawnedObject.transform.parent = anchorPoint;
        }
    }



}
