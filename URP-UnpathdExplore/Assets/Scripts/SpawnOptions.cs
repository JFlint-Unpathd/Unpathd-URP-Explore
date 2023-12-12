using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnAndToggle : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToSpawn;
    [SerializeField] float spawnRadius = 0.3f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Vector3> originalPositions = new List<Vector3>();
    
    // To be able to acess the variable from the socketinteractormanger script
    public List<GameObject> GetSpawnedObjects() 
    {
        return spawnedObjects;
    }

    private XRBaseInteractable interactable;

    private bool hasSpawned = false;

    private bool CheckIfParentIsSnapped() 
    {
        return SocketInteractorManager.CurrentSnappedObject == this.gameObject;
    }
    
    
    private void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();

        interactable.onHoverEntered.AddListener(OnHoverEnter);
        interactable.onSelectEntered.AddListener(OnSelectEnter);
        interactable.onSelectExited.AddListener(OnSelectExit);
    }

    private void Update()
    {
        bool isParentSnapped = CheckIfParentIsSnapped(); // Implement this method to check if parent is snapped to the socket interactor

        if (isParentSnapped)
        {
            foreach (var spawnedObject in spawnedObjects)
            {
                spawnedObject.SetActive(false); // Set children inactive if parent is snapped
            }
        }
        else
        {
            UpdateChildPositions(); // Call this method every frame to update the position of the child objects
        }
    }

        private void UpdateChildPositions()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            float angle = i * (360f / objectsToSpawn.Length);
            float spawnX = transform.position.x + spawnRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float spawnZ = transform.position.z + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
            Vector3 newPosition = new Vector3(spawnX, transform.position.y, spawnZ);

            spawnedObjects[i].transform.position = newPosition;
        }
    }

    private void OnHoverEnter(XRBaseInteractor interactor)
    {
        if (!hasSpawned)
        {
            SpawnObjects();
            hasSpawned = true;
        }
        else
        {
            ToggleSpawnedObjectsVisibility();
        }
    }

    private void SpawnObjects()
    {
        Debug.Log("Spawning objects...");
        Vector3 originalObjectPosition = transform.position;

        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            float angle = i * (360f / objectsToSpawn.Length);
            float spawnX = originalObjectPosition.x + spawnRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float spawnZ = originalObjectPosition.z + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);

            Vector3 spawnPosition = new Vector3(spawnX, originalObjectPosition.y, spawnZ);
            GameObject spawnedObject = Instantiate(objectsToSpawn[i], spawnPosition, Quaternion.identity);
            originalPositions.Add(spawnPosition);

            // Set the parent of the spawned object to be this script's transform
            spawnedObject.transform.parent = transform;
            spawnedObject.SetActive(true);
            spawnedObjects.Add(spawnedObject);
        }
    }

    private void ToggleSpawnedObjectsVisibility()
    {
        foreach (var spawnedObject in spawnedObjects)
        {
            //spawnedObject.SetActive(!spawnedObject.activeSelf);
            // Check if the object is not the one currently snapped to the socket interactor
            if (spawnedObject != SocketInteractorManager.CurrentChildSnappedObject)
            {
                spawnedObject.SetActive(!spawnedObject.activeSelf);
            }
        }
    }


     private void OnSelectEnter(XRBaseInteractor interactor)
    {
        // Check if any spawned object is currently visible before toggling
        if (spawnedObjects.Exists(obj => obj.activeSelf))
        {
            ToggleSpawnedObjectsVisibility();
        }

        // Unparent the spawned objects when the parent object is picked up
        foreach (var spawnedObject in spawnedObjects)
        {
            spawnedObject.transform.parent = null;
        }
    }

    private void OnSelectExit(XRBaseInteractor interactor)
    {

    }
}
