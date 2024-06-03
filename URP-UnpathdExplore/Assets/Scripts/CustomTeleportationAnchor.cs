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
        
        // // Disable all other content
        // foreach (CustomTeleportationAnchor anchor in FindObjectsOfType<CustomTeleportationAnchor>())
        // {
        //     if (anchor != this && anchor.associatedContent != null)
        //     {
        //         anchor.associatedContent.SetActive(false);
        //     }
        // }

        // // Enable the content associated with this anchor
        // if (associatedContent != null)
        // {
        //     associatedContent.SetActive(true);
        // }

        return true;
    }
}
