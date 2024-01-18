using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPopup : MonoBehaviour
{

    public InputActionReference toggleReference = null;
    public GameObject myGameObject;
    private void Awake()
    {
        toggleReference.action.started += Toggle;
    }

    private void OnDestroy()
    {
        toggleReference.action.started += Toggle;
    }

    private void Toggle(InputAction.CallbackContext context)
    {

        bool isActive = !myGameObject.activeSelf;
        myGameObject.SetActive(isActive);
    }
}
