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
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (isForwardArrow)
        {
            teleportationManager.TeleportToNextPod();
        }
        else
        {
            teleportationManager.TeleportToPreviousPod();
        }
    }
}
