using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRefine : MonoBehaviour
{
    private SqliteController m_databaseController;
    private SocketInteractorManager socketInteractorManager;
    private TransformKeeper transformKeeper;

    //private MapSpawnAndToggle mapSpawnAndToggle;

    [Header("Instantiated Objects")]
    public List<GameObject> refiningSceneObjects = new List<GameObject>();
    [Header("Results Scene Obj")]
    public List<GameObject> resultsSceneObjects = new List<GameObject>();
    private List<SocketInteractorManager> socketInteractorManagers = new List<SocketInteractorManager>();

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

    [Header("PlacementCircle Radius")]
    public float radius = 5f;
    

    private void Awake() {
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();  

    }

    private void Start() 
    {
        CreateInitialScene();
    }

    public void CreateInitialScene()
    {

        // Instantiate all prefabs and store references
        GameObject startingEnvInstance = InstantiatePrefab(startingEnv);
        refiningSceneObjects.Add(startingEnvInstance);

        GameObject refiningObjectsInstance = InstantiatePrefab(refiningObjects);
        refiningSceneObjects.Add(refiningObjectsInstance);

        // Find the XRRig GameObject
        GameObject xrRig = GameObject.FindWithTag("XRRig");

        if (xrRig != null)
        {
            // Instantiate socketInteractor as a child of XRRig
            GameObject socketInteractorInstance = InstantiatePrefab(socketInteractor);
            GameObject execQInstance = InstantiatePrefab(execQ);

            // Get the managers from the instance here instead of Awake
            foreach( SocketInteractorManager manager in socketInteractorInstance.GetComponentsInChildren<SocketInteractorManager>() ) 
            {
                socketInteractorManagers.Add( manager );
            }
            socketInteractorInstance.transform.SetParent( xrRig.transform, false );
            execQInstance.transform.SetParent( xrRig.transform, false ); // use the method to keep the local transform
            
            refiningSceneObjects.Add(socketInteractorInstance);

            // To be in the center when restarting
            xrRig.transform.position = Vector3.zero;
        }
        else
        {
            Debug.LogError("XRRig not found. Unable to instantiate socketInteractor.");
        }

        execQ.SetActive(true);

        ArrangeObjectsInCircle(refiningObjectsInstance.transform);

    }



    public void DestroyInitialScene()
    {
        foreach (SocketInteractorManager manager in socketInteractorManagers)
        {
            manager.ClearSnappedObjects();
        }

        foreach (GameObject obj in refiningSceneObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        execQ.SetActive(false);

        refiningSceneObjects.Clear();
        
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


        SFRMap.SetActive(true);
        //zoomObject.SetActive(false);
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

        // We need to clear the db results and previous query, this also now deletes all the result objects in the scene
        m_databaseController.ClearResourceDictandLists();
        m_databaseController.ResetQuery();

        //DestroyInstantiatedObjects();


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

    
    public void ArrangeObjectsInCircle(Transform transform)
    {
        Debug.Log("Arrangin in circle RR");
        int numberOfObjects = transform.childCount;

        float angleIncrement = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * angleIncrement * Mathf.Deg2Rad;
            Vector3 newPos = transform.position + new Vector3(Mathf.Cos(angle) * radius, 0.5f, Mathf.Sin(angle) * radius);

            // Set the position of the child object
            Transform childTransform = transform.GetChild(i);
            childTransform.position = newPos;

            // Calculate the direction vector towards the center
            Vector3 direction = Vector3.Normalize(Vector3.zero - newPos);

            // Rotate the object to face towards the center
            childTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            //Get the TransformKeeper script attached to the child prefab
            TransformKeeper transformKeeper = childTransform.GetComponent<TransformKeeper>();

            if (transformKeeper != null)
            {
                // Save the original transform after arranging the objects in the circle
                transformKeeper.SaveOriginalTransform();
                //Debug.Log("Saved CIRCLEtransform for child " + i + ": Position - " + transformKeeper.OriginalPosition + ", Rotation - " + transformKeeper.OriginalRotation);

            }

           //Debug.Log($"Setting position of child {i} to {newPos}");

        }
    }



}
