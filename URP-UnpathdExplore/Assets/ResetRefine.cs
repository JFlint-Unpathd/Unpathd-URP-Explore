using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRefine : MonoBehaviour
{
    private SqliteController m_databaseController;
    private SocketInteractorManager socketInteractorManager;
    private MapSpawnAndToggle mapSpawnAndToggle;

    public GameObject SFRMap;
    public GameObject bathymetricMap;

    public GameObject refiningObjects;
    public GameObject socketInteractor;
    public GameObject refQ;

    public GameObject zoomObject;
    public GameObject birdsEye;
    

    private void Start() {
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();
        socketInteractorManager = GetComponent<SocketInteractorManager>();
        SFRMap = GameObject.FindGameObjectWithTag("SFR");

        Destroy(refiningObjects);
        Destroy(socketInteractor);
        
        
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
            socketInteractorManager.ClearSnappedObjects();
            socketInteractorManager.ClearCurrentSnappedObjects();

        }
        
        //Destroy instantiated items
        DestroyInstantiatedObjects();
        m_databaseController.ResetQuery();
        

        if (SFRMap != null) {
        SFRMap.SetActive(false);}
        else {
            Debug.LogError("SFRMap not found!");
        }

        birdsEye.SetActive(false);
        zoomObject.SetActive(false);

        bathymetricMap.SetActive(true);
        refiningObjects.SetActive(true);
        socketInteractor.SetActive(true);

        refQ.SetActive(true);



        this.gameObject.SetActive(false);

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
