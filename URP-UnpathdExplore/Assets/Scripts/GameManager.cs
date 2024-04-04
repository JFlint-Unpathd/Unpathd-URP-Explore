using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioManager audioManager;
    public FontSizeManager fontSizeManager;
    public UIColorManager uiColorManager;

    void Awake()
    {
        DontDestroyOnLoad(audioManager.gameObject);
        DontDestroyOnLoad(fontSizeManager.gameObject);
        DontDestroyOnLoad(uiColorManager.gameObject);
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
}
