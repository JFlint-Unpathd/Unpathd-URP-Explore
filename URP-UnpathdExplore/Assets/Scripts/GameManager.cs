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

    void Start()
    {
        // Check if the current scene is the first scene
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // Disable the LocomotionSystem in the first scene
            GameObject locomotionSystem = GameObject.Find("Locomotion System");
            if (locomotionSystem != null)
            {
                locomotionSystem.SetActive(false);
            }
        }
        else
        {
            // Enable the LocomotionSystem in subsequent scenes
            LocomotionSystem.SetActive(true);
        }

        xrrig = GameObject.Find("xrrig");

        // Check if the xrrig GameObject exists
        if (xrrig != null)
        {
            // Reset the xrrig position to Vector3.0 when the scene starts
            xrrig.transform.position = Vector3.zero;
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
