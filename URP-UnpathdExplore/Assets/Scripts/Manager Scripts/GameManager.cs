using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public GameObject xrrig;
    public GameObject locomotionSystem;

    void Awake()
    {
        //Debug.Log("Awake called. Current instance: " + instance);
        
        if(instance != null)
        {
            Debug.Log("Instance already exists. Destroying gameObject.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("No existing instance. Setting this as instance.");
            instance = this;
            PersistanceClass.DontDestroyOnLoad(gameObject);
        }
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

     void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Loaded" + scene.name);
        SetupScene();
    }

      void SetupScene()
    {
        // Find the xrrig GameObject
        xrrig = GameObject.Find("xrrig");

        // Reset the xrrig position to Vector3.zero when the scene starts
        if (xrrig != null)
        {
            xrrig.transform.position = Vector3.zero;
        }

         // Find the LocomotionSystem GameObject
        locomotionSystem = GameObject.Find("Locomotion System");

        // Check if the LocomotionSystem is found
        if (locomotionSystem == null)
        {
            Debug.LogWarning("Locomotion System not found in the scene.");
            return;
        }

        // Check if this is the first or second scene
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 0 || sceneIndex == 1)
        {
            // Disable the LocomotionSystem in the first or second scene
            locomotionSystem.SetActive(false);
        }
        else
        {
            // Enable the LocomotionSystem in subsequent scenes
            locomotionSystem.SetActive(true);
        }
    }

    public void QuitGame()
    {
        
        Application.Quit();
    }

    public void RestartGame()
    {
    
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    
    public void LoadPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex - 1);
    }


}
