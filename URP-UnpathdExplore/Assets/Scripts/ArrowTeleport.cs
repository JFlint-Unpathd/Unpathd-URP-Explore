using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ArrowTeleport : MonoBehaviour
{
    public TeleportationManager teleportationManager; 
    public bool isForwardArrow; 
    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        
        interactable.selectEntered.AddListener(OnSelectEntered);
        //interactable.hoverEntered.AddListener(OnHoverEntered);
        //interactable.hoverExited.AddListener(OnHoverExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        OnHoverEntered(null);
        if (isForwardArrow)
        {
            teleportationManager.TeleportToNextPod();
        }
        else
        {
            teleportationManager.TeleportToPreviousPod();
        }
    }

     private void OnHoverEntered(HoverEnterEventArgs args)
    {
        teleportationManager.OnButtonHoverEnter();
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        teleportationManager.OnButtonHoverExit();
    }
}
