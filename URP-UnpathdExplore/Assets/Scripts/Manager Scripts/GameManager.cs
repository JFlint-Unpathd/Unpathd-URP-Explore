using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class GameManager : MonoBehaviour
{
    static GameManager instance;

    private GameObject xrrig;
    public GameObject LocomotionSystem;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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

        // Check if this is the first scene
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // Disable the LocomotionSystem in the first scene
            if (LocomotionSystem != null)
            {
                LocomotionSystem.SetActive(false);
            }
        }
        else
        {
            // Enable the LocomotionSystem in subsequent scenes
            if (LocomotionSystem != null)
            {
                LocomotionSystem.SetActive(true);
            }
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
