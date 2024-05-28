using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
        
        // Get the name of the next scene
        string nextSceneName = SceneManager.GetSceneByBuildIndex(currentSceneIndex + 1).name;
        
        // Call the HandleSceneAudio function from the VoiceoverManager to play the corresponding audio
        VoiceoverManager.instance.HandleSceneAudio(nextSceneName);
    }
}
