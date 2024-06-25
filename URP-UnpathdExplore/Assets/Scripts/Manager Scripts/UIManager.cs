using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    void Awake()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // If we are in the first scene (index 0), do not persist the UIManager
        if (currentSceneIndex == 0)
        {
            // Make sure to destroy any existing instance in the first scene
            if (instance != null && instance != this)
            {
                Destroy(instance.gameObject);
            }
            instance = this;
            return;
        }

        // Otherwise, handle the singleton instance and persist from the second scene onward
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
