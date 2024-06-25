using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

public class LoadNextSceneWithCoroutine : MonoBehaviour
{

    //public LocomotionProvider locomotionProvider;

    void Start()
    {
        StartCoroutine(WaitAndLoadNextScene());
    }

    IEnumerator WaitAndLoadNextScene()
    {
        // Wait until introClipsFinished is true
        //yield return new WaitUntil(() => VoiceoverManager.instance.introClipsFinished);

        // Wait for an additional 3 seconds
        yield return new WaitForSeconds(5f);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1); // load the next scene
    }

}


