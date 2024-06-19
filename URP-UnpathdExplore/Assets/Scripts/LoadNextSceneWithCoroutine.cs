// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Collections;
// using System.Collections.Generic;

// public class LoadNextSceneWithCoroutine : MonoBehaviour
// {

//     void Start()
//     {
//         StartCoroutine(WaitAndLoadNextScene());
//     }

//     IEnumerator WaitAndLoadNextScene()
//     {
//         yield return new WaitForSeconds(5);

//         int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
//         SceneManager.LoadScene(currentSceneIndex + 1); // load the next scene
//     }

// }


using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadNextSceneWithCoroutine : MonoBehaviour
{
    private SceneLoadingVignetteProvider vignetteProvider;

    void Start()
    {
        // Get the SceneLoadingVignetteProvider component on the same GameObject
        vignetteProvider = GetComponent<SceneLoadingVignetteProvider>();

        StartCoroutine(WaitAndLoadNextScene());
    }

    IEnumerator WaitAndLoadNextScene()
    {
        yield return new WaitForSeconds(5);

        // Example: Update vignette parameters before loading next scene
        vignetteProvider.UpdateVignetteParameters(0.0f); // Initial loading progress

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1); // Load the next scene

        // Example: Simulate loading progress and update vignette parameters
        float progress = 0.0f;
        while (progress < 1.0f)
        {
            progress += Time.deltaTime / 5.0f; // Simulate 5 seconds loading time
            vignetteProvider.UpdateVignetteParameters(progress); // Update vignette parameters
            yield return null;
        }

        // Ensure vignette is fully eased out after loading
        vignetteProvider.UpdateVignetteParameters(1.0f); // Max progress (fully loaded)
    }
}
