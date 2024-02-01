using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRefine : MonoBehaviour
{
    private SqliteController m_databaseController;
    private SocketInteractorManager socketInteractorManager;
    private MapSpawnAndToggle mapSpawnAndToggle;

    [Header("Instantiated Objects")]
    public List<GameObject> refiningSceneObjects = new List<GameObject>();
    [Header("Results Scene Obj")]
    public List<GameObject> resultsSceneObjects = new List<GameObject>();

    [Header("Maps")]
    public GameObject SFRMap;
    public GameObject bathymetricMap;

    [Header("Prefabs")]
    public GameObject refiningObjects;
    public GameObject socketInteractor;

    [Header("Scene Objects")]
    public GameObject execQ;
    public GameObject reRef;

    [Header("Other Prefabs")]
    public GameObject zoomObject;
    public GameObject birdsEye;
    

    private void Start() {
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();  
        //socketInteractorManager = socketInteractor.GetComponentInChildren<SocketInteractorManager>();

        //SFRMap = GameObject.FindGameObjectWithTag("SFR");
  
    }

    
    public void CreateInitialScene()
    {
        // Instantiate all prefabs and store references
        GameObject bathymetricMapInstance = InstantiatePrefab(bathymetricMap);
        refiningSceneObjects.Add(bathymetricMapInstance);

        GameObject refiningObjectsInstance = InstantiatePrefab(refiningObjects);
        refiningSceneObjects.Add(refiningObjectsInstance);

        GameObject socketInteractorInstance = InstantiatePrefab(socketInteractor);
        refiningSceneObjects.Add(socketInteractorInstance);

        execQ.SetActive(true);
    }


    public void DestroyInitialScene()
    {
        // Destroy or deactivate all instantiated objects
        foreach (GameObject obj in refiningSceneObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        refiningSceneObjects.Clear();
        Debug.Log("Should have destroyed");

        execQ.SetActive(false);

    }

    private GameObject InstantiatePrefab(GameObject prefab)
    {
        if (prefab != null)
        {
            return Instantiate(prefab);
        }
        else
        {
            Debug.LogError("Prefab is null. Cannot instantiate.");
            return null;
        }
    }

    public void CreateResultsScene()
    {
        GameObject birdsEyeInstance = InstantiatePrefab(birdsEye);
        resultsSceneObjects.Add(birdsEye);

        GameObject SFRMap = InstantiatePrefab(SFRMap);
        resultsSceneObjects.Add(SFRMap);

        
        reRef.SetActive(true);
        zoomObject.SetActive(true);

    }

    public void DestroyResultsScene()
    {
        
        // Destroy or deactivate all results scene objects
        foreach (GameObject obj in resultsSceneObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        resultsSceneObjects.Clear();
        Debug.Log("Should have destroyed");

        reRef.SetActive(false);
        zoomObject.SetActive(false);
    }

    
    public void ResetRefineSearch()
    {

        // Clear results in the database controller
        List<UnpathResource> allResults = m_databaseController.GetAllQResults();
        allResults.Clear();

        // Clear the dictionary in SqliteController
        m_databaseController.ClearResourceDictandLists();

        // Clear lists and dictionaries in MapSpawnAndToggle
        if (mapSpawnAndToggle != null)
        {
            mapSpawnAndToggle.ClearListsAndDictionaries();
        }

        // Access SocketInteractorManager and clear snapped objects lists
        if (socketInteractorManager != null)
        {
            socketInteractorManager.ResetSocketInteractor();
            
        }

        SFRMap = GameObject.FindGameObjectWithTag("SFR");
        
        //Destroy instantiated items from results Q
        DestroyInstantiatedObjects();
        m_databaseController.ResetQuery();
        

        if (SFRMap != null) {
        SFRMap.SetActive(false);}
        else {
            Debug.LogError("SFRMap not found!");
        }

        DestroyResultsScene();
        CreateInitialScene();


    }

    private void DestroyInstantiatedObjects()
    {
        // Get all GameObjects with UnpathResource component
        UnpathResource[] refiningObjects = FindObjectsOfType<UnpathResource>();

        // Destroy each instantiated GameObject
        foreach (UnpathResource obj in refiningObjects)
        {
            Destroy(obj.gameObject);
        }
    }

}
