using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadNextSceneWithCoroutine : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndLoadNextScene());
    }

    IEnumerator WaitAndLoadNextScene()
    {
        yield return new WaitForSeconds(3); // wait for 3 seconds

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1); // load the next scene
    }
}
