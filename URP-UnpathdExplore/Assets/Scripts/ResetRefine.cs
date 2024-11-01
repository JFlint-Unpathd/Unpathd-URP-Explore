using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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

    private GameObject execQInstance;
    private GameObject reRefInstance;

    private Vector3 originalExecQPosition;
    private Quaternion originalExecQRotation;

    private Vector3 originalReRefPosition;
    private Quaternion originalReRefRotation;

    [Header("Other Prefabs")]
    public GameObject zoomObject;
    public GameObject birdsEye;
    private GameObject birdsEyeInstance;
    //public GameObject groundFog;

    [Header("PlacementCircle Radius")]
    public float radius = 5f;

    private bool isReaderDone = false;
    private bool resultsAudioPlayed = false;
    

    private void Awake() {
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();  

        originalExecQPosition = new Vector3(1, 0.15f, 2);
        originalExecQRotation = Quaternion.Euler(0, 33, 0);

        originalReRefPosition = new Vector3(1, 0.15f, 2);
        originalReRefRotation = Quaternion.Euler(0, 33, 0);

    }

    private void Start() 
    {
        CreateInitialScene();
        
    }

    public void CreateInitialScene()
    {
        
       // groundFog.SetActive(true);

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

            if (execQInstance == null)
            {
                execQInstance = InstantiatePrefab(execQ);
                execQInstance.transform.position = originalExecQPosition;
                execQInstance.transform.rotation = originalExecQRotation;
                
            }
            else
            {
                // Reset the position and rotation if the object already exists
                ResetTransform(execQInstance, originalExecQPosition, originalExecQRotation);
            }


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

        execQInstance.SetActive(true);
        ArrangeObjectsInCircle(refiningObjectsInstance.transform);
        ResetTransform(execQInstance, originalExecQPosition, originalExecQRotation);

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

        refiningSceneObjects.Clear();
        //groundFog.SetActive(false);
        
    }

    private GameObject InstantiatePrefab(GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab);
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.isKinematic = true;  // Set the Rigidbody component to kinematic
            }
            else
            {
                Debug.LogWarning("No Rigidbody component found on the instantiated prefab. Kinematic state not set.");
            }

            return instance;
        }
        else
        {
            Debug.LogError("Prefab is null. Cannot instantiate.");
            return null;
        }
    }

    public void CreateResultsScene()
    {
        
        execQInstance.SetActive(false);
        // Find the XRRig GameObject
        GameObject xrRig = GameObject.FindWithTag("XRRig");

        if (!resultsAudioPlayed) 
        {
            VoiceoverManager.Stop();
            VoiceoverManager.instance.PlayResultsAudio();
            resultsAudioPlayed = true;
        }

        if (reRefInstance == null)
        {
            reRefInstance = InstantiatePrefab(reRef);
            reRefInstance.transform.position = originalReRefPosition;
            reRefInstance.transform.rotation = originalReRefRotation;
            
        }
        else
        {
            // Reset the position and rotation if the object already exists
            ResetTransform(execQInstance, originalExecQPosition, originalExecQRotation);
        }
        
        if (xrRig != null)
        {
            // To be in the center when restarting
            xrRig.transform.position = Vector3.zero;
            reRefInstance.transform.SetParent( xrRig.transform, false );

        }

        if (resultsSceneObjects.Count == 0)
        {
            SFRMap = GameObject.FindGameObjectWithTag("SFR");

            birdsEyeInstance = InstantiatePrefab(birdsEye);
            resultsSceneObjects.Add(birdsEyeInstance);

        }

        DisableBirdsEye();

        // Activate scene objects if they exist
        foreach (GameObject obj in resultsSceneObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }


        SFRMap.SetActive(true);
        reRefInstance.SetActive(true);
        ResetTransform(reRefInstance, originalReRefPosition, originalReRefRotation);
        //zoomObject.SetActive(true);

    }

    public void DestroyResultsScene()
    {
        m_databaseController.StopQuery();
        // Destroy or deactivate all results scene objects
        foreach (GameObject obj in resultsSceneObjects)
        {
             if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        
        resultsSceneObjects.Clear();
        VoiceoverManager.Stop();
        Debug.Log("Should have destroyed resultssceneobj");

        // Deactivate other scene objects
        if (SFRMap != null)
        {
            SFRMap.SetActive(false);
        }
        if (reRefInstance != null)
        {
            reRefInstance.SetActive(false);
        }
        if (zoomObject != null)
        {
            zoomObject.SetActive(false);
        }

        // We need to clear the db results and previous query, this also now deletes all the result objects in the scene
        m_databaseController.ClearResourceDictandLists();
        m_databaseController.ResetQuery();

        InfoPanel.Hide();

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

            }
        }
    }

    private void ResetTransform(GameObject gameObject, Vector3 originalPosition, Quaternion originalRotation)
    {
        if (gameObject != null)
        {
            gameObject.transform.localPosition = originalPosition;
            gameObject.transform.localRotation = originalRotation;
        }
        else
        {
            Debug.LogError("GameObject is null. Cannot reset transform.");
        }
    }

    
    // New methods for enabling and disabling birdsEye view
    private void DisableBirdsEye()
    {
        if (birdsEyeInstance != null) 
        {
            var rb = birdsEyeInstance.GetComponent<Rigidbody>();
            var interactable = birdsEyeInstance.GetComponent<XRSimpleInteractable>();

            if (rb != null) rb.isKinematic = true;
            if (interactable != null) interactable.enabled = false;
        }
        else
        {
            Debug.Log("BirdsEyeInstance not found");
        }
    }

    private void EnableBirdsEye()
    {
        if (birdsEyeInstance != null) 
        {
            var rb = birdsEyeInstance.GetComponent<Rigidbody>();
            var interactable = birdsEyeInstance.GetComponent<XRSimpleInteractable>();

            if (rb != null) rb.isKinematic = true;
            if (interactable != null) interactable.enabled = true;
        }
        else
        {
            Debug.Log("BirdsEyeInstance not found");
        }
    }

    // Method to be called by SqliteController when processing is done
    public void OnProcessingDone()
    {
        isReaderDone = true;
        EnableBirdsEye();
    }


}
