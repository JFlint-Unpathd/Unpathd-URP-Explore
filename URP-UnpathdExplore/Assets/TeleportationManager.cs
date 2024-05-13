using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour

{public GameObject[] teleportationPods;
    public GameObject[] podContents;
    private int currentPodIndex = 0;
    private TeleportationProvider teleportationProvider;

    void Start()
    {
        // Initialize visibility of pod contents
        UpdatePodContentsVisibility();

   // Initialize visibility of pod contents
        UpdatePodContentsVisibility();

        // Find the TeleportationProvider component
        teleportationProvider = FindObjectOfType<TeleportationProvider>();

        if (teleportationProvider == null)
        {
            Debug.LogError("TeleportationProvider component not found in the scene.");
        }
        else
        {
            // Subscribe the TeleportToNextPod method to the teleportationComplete event
            teleportationProvider.teleportationComplete += HandleTeleportationComplete;
        }

    }

        // Event handler for teleportationComplete event
    private void HandleTeleportationComplete(TeleportationProvider provider, TeleportRequest request)
    {
        TeleportToNextPod();
    }

    public void TeleportToNextPod()
    {
        // Increment the current pod index (with wrap-around)
        currentPodIndex = (currentPodIndex + 1) % teleportationPods.Length;

        // Move XR rig to the position of the next pod
        Vector3 newPosition = teleportationPods[currentPodIndex].transform.position;
        teleportationProvider.QueueTeleportRequest(new TeleportRequest { destinationPosition = newPosition });

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
