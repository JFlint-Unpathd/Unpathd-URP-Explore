using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRefine : MonoBehaviour
{
    private SqliteController m_databaseController;
    private SocketInteractorManager socketInteractorManager;
    private MapSpawnAndToggle mapSpawnAndToggle;

    [Header("Instantiated Objects")]
    public List<GameObject> instantiatedObjects = new List<GameObject>();

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

    // public void CreateInitialScene()
    // {
    //     bathymetricMap.GetComponent<PrefabInstantiator>().InstantiatePrefab();
    //     socketInteractor.GetComponent<PrefabInstantiator>().InstantiatePrefab();
    //     refiningObjects.GetComponent<PrefabInstantiator>().InstantiatePrefab();
    //     //execQ.GetComponent<PrefabInstantiator>().InstantiatePrefab();
    //     execQ.SetActive(true);
       
    // }

    public void CreateInitialScene()
    {
        // Instantiate all prefabs and store references
        GameObject bathymetricMapInstance = InstantiatePrefab(bathymetricMap);
        instantiatedObjects.Add(bathymetricMapInstance);

        GameObject refiningObjectsInstance = InstantiatePrefab(refiningObjects);
        instantiatedObjects.Add(refiningObjectsInstance);

        GameObject socketInteractorInstance = InstantiatePrefab(socketInteractor);
        instantiatedObjects.Add(socketInteractorInstance);

        execQ.SetActive(true);
    }

    public void DestroyInitialScene()
    {
        // Check if the bathymetricMap is instantiated and not the original prefab
        if (bathymetricMap != null && bathymetricMap.scene.IsValid())
        {
            Destroy(bathymetricMap); // Destroy the instantiated GameObject
        }

        if (refiningObjects != null && refiningObjects.scene.IsValid())
        {
            Destroy(refiningObjects); // Destroy the instantiated GameObject
        }

        if (socketInteractor != null && socketInteractor.scene.IsValid())
        {
            Destroy(socketInteractor); // Destroy the instantiated GameObject
        }

        
        
        //Destroy(execQ);
        execQ.SetActive(false);
        
    }

    public void ResetScene()
    {
        // Destroy or deactivate all instantiated objects
        foreach (GameObject obj in instantiatedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        instantiatedObjects.Clear();
        Debug.Log("Should have destroyed");

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
        birdsEye.GetComponent<PrefabInstantiator>().InstantiatePrefab();
        //birdsEye.SetActive(true);
        //reRef.GetComponent<PrefabInstantiator>().InstantiatePrefab();
        reRef.SetActive(true);
        zoomObject.GetComponent<PrefabInstantiator>().InstantiatePrefab();
        //zoomObject.SetActive(true);

        SFRMap.GetComponent<PrefabInstantiator>().InstantiatePrefab();
    }

    public void DestroyResultsScene()
    {
        
        Destroy(birdsEye);
        //birdsEye.SetActive(false);
        //Destroy(reRef);
        reRef.SetActive(false);
        Destroy(zoomObject);
        //zoomObject.SetActive(false);

        Destroy(SFRMap);
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
        UnpathResource[] instantiatedObjects = FindObjectsOfType<UnpathResource>();

        // Destroy each instantiated GameObject
        foreach (UnpathResource obj in instantiatedObjects)
        {
            Destroy(obj.gameObject);
        }
    }

}
