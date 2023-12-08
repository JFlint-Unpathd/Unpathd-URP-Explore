using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnAndToggle : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToSpawn;
    [SerializeField] float spawnRadius = 1.0f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private XRBaseInteractable interactable;
    private bool hasSpawned = false;
    
    private void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.onHoverEntered.AddListener(OnHoverEnter);
        interactable.onSelectEntered.AddListener(OnSelectEnter);
        interactable.onSelectExited.AddListener(OnSelectExit);
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
            spawnedObject.SetActive(true);
            spawnedObjects.Add(spawnedObject);
        }
    }

    private void ToggleSpawnedObjectsVisibility()
    {
        foreach (var spawnedObject in spawnedObjects)
        {
            spawnedObject.SetActive(!spawnedObject.activeSelf);
        }
    }
     private void OnSelectEnter(XRBaseInteractor interactor)
    {
        // Check if any spawned object is currently visible before toggling
        if (spawnedObjects.Exists(obj => obj.activeSelf))
        {
            ToggleSpawnedObjectsVisibility();
        }
    }

    private void OnSelectExit(XRBaseInteractor interactor)
    {
        // Optionally handle select exit events if needed
    }
}
