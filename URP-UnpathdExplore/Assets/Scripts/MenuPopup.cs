using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPopup : MonoBehaviour
{

    public InputActionReference toggleReference = null;
    public GameObject settingsMenu;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        settingsMenu.SetActive(false);
        toggleReference.action.started += Toggle;

        //originalPosition = settingsMenu.transform.position;
        originalPosition = settingsMenu.transform.position + new Vector3(0, 0.5f, 0);
        originalRotation = settingsMenu.transform.rotation;


    }

    private void Start()
    {
        
        //StartCoroutine(DisableInGameMenuCoroutine());
    }

    IEnumerator DisableInGameMenuCoroutine()
    {
        
        yield return null;

        settingsMenu = GameObject.FindGameObjectWithTag("InGameMenu");
        if (settingsMenu == null)
        {
            Debug.Log("No GameObject with tag 'InGameMenu' found.");
        }
        else
        {
            settingsMenu.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        toggleReference.action.started -= Toggle;
    }

    private void Toggle(InputAction.CallbackContext context)
    {

        bool isActive = !settingsMenu.activeSelf;

        if(isActive)
        {
            // Save the original position
            originalPosition = settingsMenu.transform.position;
            originalRotation = settingsMenu.transform.rotation;

            // Position the menu 3 units in front of the camera on the Z axis
            //settingsMenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;       
            settingsMenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3 + new Vector3(0, 0.5f, 0);


            // Calculate the rotation to face the camera (only rotating around the y-axis)
            Vector3 toCameraDirection = Camera.main.transform.position - settingsMenu.transform.position;
            toCameraDirection.y = 0f; // Ignore vertical component

            Quaternion targetRotation = Quaternion.LookRotation(toCameraDirection);
            settingsMenu.transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        }
        else
        {
            // Reset to the original position
            settingsMenu.transform.position = originalPosition;
            settingsMenu.transform.rotation = originalRotation;
        }
        
        settingsMenu.SetActive(isActive);
    }
}
