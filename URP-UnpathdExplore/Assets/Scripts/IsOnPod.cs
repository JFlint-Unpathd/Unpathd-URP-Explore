using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IsOnPod : MonoBehaviour
{
    private TeleportationAnchor teleportationAnchor;
    public ControllerButtonGlow controllerButtonGlow;

private void Awake()
    {
        teleportationAnchor = GetComponent<TeleportationAnchor>();
        
        GameObject controllerObject = GameObject.FindWithTag("DemoController");
        if (controllerObject != null)
        {
            controllerButtonGlow = controllerObject.GetComponent<ControllerButtonGlow>();
        }

        if (controllerButtonGlow != null)
        {
            teleportationAnchor.selectEntered.AddListener(HandleSelectEntered);
            teleportationAnchor.selectExited.AddListener(HandleSelectExited);
        }
        else
        {
            Debug.LogError("ControllerButtonGlow script not found!");
        }       
    }

    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        controllerButtonGlow.isOnPod = true;
    }

    private void HandleSelectExited(SelectExitEventArgs args)
    {
        controllerButtonGlow.isOnPod = false;
    }

}
