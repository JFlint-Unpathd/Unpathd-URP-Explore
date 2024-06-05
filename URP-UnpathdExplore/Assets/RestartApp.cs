using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    [SerializeField]
    private InputActionReference restartButtonReference;

    private void OnEnable()
    {
        restartButtonReference.action.performed += ctx => SceneManager.LoadScene(0);
    }

    private void OnDisable()
    {
        restartButtonReference.action.performed -= ctx => SceneManager.LoadScene(0);
    }
}
