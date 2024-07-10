using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomTeleportationAnchor : TeleportationAnchor
{
    public GameObject associatedContent;

    protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
    {
        if (teleportAnchorTransform == null)
            return false;

        teleportRequest.destinationPosition = teleportAnchorTransform.position;
        teleportRequest.destinationRotation = teleportAnchorTransform.rotation;

        TeleportationManager.SetPodIndex( transform.GetSiblingIndex() );
        
        return true;
    }
}
