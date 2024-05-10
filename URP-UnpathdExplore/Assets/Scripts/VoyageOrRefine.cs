using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class VoyageOrRefine : MonoBehaviour
{
    private string tag1 = "Voyages";
    public float spawnRadius = 1;

    [SerializeField] GameObject[] voyageOptions;
    public Transform anchorPoint; 
    private XRGrabInteractable grabInteractable;

    private Vector3 parentPosition;
    private Quaternion parentRotation;

    private bool hasSpawned = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
    }

    void Start()
    {
        parentPosition = transform.position;
        parentRotation = transform.rotation;
    }


    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        GameObject item = args.interactable.gameObject;
        
        if (item.tag == tag1 && !hasSpawned)
        {
            // Activate the four other voyage options
            SpawnVoyageOptions();
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

}
