using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoyageMenuSceneLoader : MonoBehaviour
{
    public void LoadVoyageMenuScene()
    {
        SceneManager.LoadScene("Voyages Menu");
    }
}
