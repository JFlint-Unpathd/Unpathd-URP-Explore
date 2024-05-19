using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour

{
    public TeleportationProvider teleportationProvider; 
    public GameObject[] teleportationPods;
    public GameObject[] podContents;
    private int currentPodIndex = 0;


    void Start()
    {

        UpdatePodContentsVisibility();

    }


    public void TeleportToNextPod()
    {
        // Increment the current pod index (with wrap-around)
        currentPodIndex = (currentPodIndex + 1) % teleportationPods.Length;

        // Get the next pod's teleportation anchor
        TeleportationAnchor nextPodTeleportationAnchor = teleportationPods[currentPodIndex].GetComponent<TeleportationAnchor>();
        
        // Generate a new teleport request
        TeleportRequest teleportRequest = new TeleportRequest
        {
            destinationPosition = nextPodTeleportationAnchor.teleportAnchorTransform.position,
            destinationRotation = nextPodTeleportationAnchor.teleportAnchorTransform.rotation
        };

        // Queue the teleportation request
        teleportationProvider.QueueTeleportRequest(teleportRequest);

        // Update the visibility of pod contents
        UpdatePodContentsVisibility();
    }

    public void TeleportToPreviousPod()
    {
        // Decrement the current pod index (with wrap-around)
        currentPodIndex = (currentPodIndex - 1 + teleportationPods.Length) % teleportationPods.Length;

        // Get the previous pod's teleportation anchor
        TeleportationAnchor previousPodTeleportationAnchor = teleportationPods[currentPodIndex].GetComponent<TeleportationAnchor>();

        // Generate a new teleport request
        TeleportRequest teleportRequest = new TeleportRequest
        {
            destinationPosition = previousPodTeleportationAnchor.teleportAnchorTransform.position,
            destinationRotation = previousPodTeleportationAnchor.teleportAnchorTransform.rotation
        };

        // Queue the teleportation request
        teleportationProvider.QueueTeleportRequest(teleportRequest);

        // Update the visibility of pod contents
        UpdatePodContentsVisibility();
    }


    // Update the visibility of pod contents based on the current pod index
    void UpdatePodContentsVisibility()
    {
        for (int i = 0; i < podContents.Length; i++)
        {
            podContents[i].SetActive(i == currentPodIndex);
        }
    }
    
}
