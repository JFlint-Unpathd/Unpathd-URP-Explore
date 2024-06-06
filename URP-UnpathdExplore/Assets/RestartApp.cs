using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{

    [SerializeField]
    private InputActionReference restartButtonReference;

    private void Awake()
    {
        Object.DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        
        restartButtonReference.action.performed += ResetApplication;
        Debug.Log("bla");
    }

    private void ResetApplication(InputAction.CallbackContext ctx)
    {
        PersistanceClass.DestroyAll();
        SceneManager.LoadScene(0);
    }

    private void OnDisable()
    {
        restartButtonReference.action.performed -= ResetApplication;
    }
}
