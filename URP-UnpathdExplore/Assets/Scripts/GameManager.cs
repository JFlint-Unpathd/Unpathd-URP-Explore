using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    private GameObject xrrig;

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

    void Update()
    {
        if (Input.GetKeyDown("2"))
        {
            SceneManager.LoadScene("Voyages");
        }
        else if (Input.GetKeyDown("1"))
        {
            SceneManager.LoadScene("DatabaseSearch");
        }
    }
}
