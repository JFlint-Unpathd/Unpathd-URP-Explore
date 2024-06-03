using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    public TeleportationProvider teleportationProvider;
    public GameObject[] teleportationPods;
    public GameObject[] podContents;
    private int currentPodIndex = 0;

    public static TeleportationManager _instance;

    public static void SetPodIndex(int index)
    {
        _instance.currentPodIndex = index;
        _instance.UpdatePodContentsVisibility();
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        UpdatePodContentsVisibility();
    }

    public void TeleportToNextPod()
    {
        // Increment the current pod index (with wrap-around)
        currentPodIndex = (currentPodIndex + 1) % teleportationPods.Length;
        TeleportToCurrentPod();
    }

    public void TeleportToPreviousPod()
    {
        // Decrement the current pod index (with wrap-around)
        currentPodIndex = (currentPodIndex - 1 + teleportationPods.Length) % teleportationPods.Length;
        TeleportToCurrentPod();
    }

    private void TeleportToCurrentPod()
    {
        // Get the current pod's teleportation anchor
        TeleportationAnchor currentPodTeleportationAnchor = teleportationPods[currentPodIndex].GetComponent<TeleportationAnchor>();

        // Ensure the teleportation anchor is valid
        if (currentPodTeleportationAnchor != null)
        {
            // Generate a new teleport request
            TeleportRequest teleportRequest = new TeleportRequest
            {
                destinationPosition = currentPodTeleportationAnchor.teleportAnchorTransform.position,
                destinationRotation = currentPodTeleportationAnchor.teleportAnchorTransform.rotation
            };

            // Queue the teleportation request
            teleportationProvider.QueueTeleportRequest(teleportRequest);
        }

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
