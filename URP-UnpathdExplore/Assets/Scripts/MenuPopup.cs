using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPopup : MonoBehaviour
{

    public InputActionReference toggleReference = null;
    public GameObject settingsMenu;
    private Vector3 originalPosition;

    private void Awake()
    {
        toggleReference.action.started += Toggle;
        originalPosition = settingsMenu.transform.position;
    }

    private void OnDestroy()
    {
        toggleReference.action.started += Toggle;
    }

    private void Toggle(InputAction.CallbackContext context)
    {

        bool isActive = !settingsMenu.activeSelf;

        if(isActive)
        {
            // Save the original position
            originalPosition = settingsMenu.transform.position;

            // Position the menu 3 units in front of the camera on the Z axis
            settingsMenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;
        }
        else
        {
            // Reset to the original position
            settingsMenu.transform.position = originalPosition;
        }
        
        settingsMenu.SetActive(isActive);
    }
}
