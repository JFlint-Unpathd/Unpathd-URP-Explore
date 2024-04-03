using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRefine : MonoBehaviour
{
    private SqliteController m_databaseController;
    private SocketInteractorManager socketInteractorManager;
   
    //private MapSpawnAndToggle mapSpawnAndToggle;

    [Header("Instantiated Objects")]
    public List<GameObject> refiningSceneObjects = new List<GameObject>();
    [Header("Results Scene Obj")]
    public List<GameObject> resultsSceneObjects = new List<GameObject>();

    [Header("Maps")]
    public GameObject SFRMap;
    public GameObject startingEnv;

    [Header("Prefabs")]
    public GameObject refiningObjects;
    public GameObject socketInteractor;

    [Header("Scene Objects")]
    public GameObject execQ;
    public GameObject reRef;

    [Header("Other Prefabs")]
    public GameObject zoomObject;
    public GameObject birdsEye;
    

    private void Awake() {
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();  
        //socketInteractorManager = socketInteractor.GetComponentInChildren<SocketInteractorManager>();
  
    }

    
    public void CreateInitialScene()
    {
        // Instantiate all prefabs and store references
        GameObject startingEnvInstance = InstantiatePrefab(startingEnv);
        refiningSceneObjects.Add(startingEnvInstance);

        GameObject refiningObjectsInstance = InstantiatePrefab(refiningObjects);
        refiningSceneObjects.Add(refiningObjectsInstance);

        GameObject socketInteractorInstance = InstantiatePrefab(socketInteractor);
        refiningSceneObjects.Add(socketInteractorInstance);

        execQ.SetActive(true);

          // Get the CircleObjectPlacer component from the refiningObjectsInstance and call ArrangeObjectsInCircle
        CircleObjectPlacer circleObjectPlacer = refiningObjectsInstance.GetComponent<CircleObjectPlacer>();
        if (circleObjectPlacer != null)
        {
            circleObjectPlacer.ArrangeObjectsInCircle();
        }
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
        if (resultsSceneObjects.Count == 0)
        {
            SFRMap = GameObject.FindGameObjectWithTag("SFR");

            GameObject birdsEyeInstance = InstantiatePrefab(birdsEye);
            resultsSceneObjects.Add(birdsEyeInstance);
        }

        // Activate scene objects if they exist
        foreach (GameObject obj in resultsSceneObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        //SFRMap = GameObject.FindGameObjectWithTag("SFR");

        //GameObject birdsEyeInstance = InstantiatePrefab(birdsEye);
        //resultsSceneObjects.Add(birdsEye);

        SFRMap.SetActive(true);
        reRef.SetActive(true);
        zoomObject.SetActive(true);

    }

    public void DestroyResultsScene()
    {
        
        // Destroy or deactivate all results scene objects
        foreach (GameObject obj in resultsSceneObjects)
        {
            // if (obj != null)
            // {
            //     //Destroy(obj);
            //     DestroyImmediate(obj);
            // }

             if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        
        resultsSceneObjects.Clear();
        Debug.Log("Should have destroyed resultssceneobj");

        // Deactivate other scene objects
        if (SFRMap != null)
        {
            SFRMap.SetActive(false);
        }
        if (reRef != null)
        {
            reRef.SetActive(false);
        }
        if (zoomObject != null)
        {
            zoomObject.SetActive(false);
        }

        // SFRMap.SetActive(false);
        // reRef.SetActive(false);
        // zoomObject.SetActive(false);
    }

    
    public void ResetRefineSearch()
    {

        // Clear results in the database controller
        List<UnpathResource> allResults = m_databaseController.GetAllQResults();
        allResults.Clear();

        // Clear the dictionary in SqliteController
        m_databaseController.ClearResourceDictandLists();

        // Clear lists and dictionaries in MapSpawnAndToggle
        // if (mapSpawnAndToggle != null)
        // {
        //     mapSpawnAndToggle.ClearListsAndDictionaries();
        // }

        // Access SocketInteractorManager and clear snapped objects lists
        if (socketInteractorManager != null)
        {
            socketInteractorManager.ResetSocketInteractor();
            
        }


        //Destroy instantiated items from results Q from sqlite controller script
        DestroyInstantiatedObjects();
        m_databaseController.ResetQuery();
        
        
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
