using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    public TeleportationProvider teleportationProvider;
    public GameObject[] teleportationPods;
    public GameObject[] podContents;
    public GameObject[] podPlaceholders;
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
        // Increment the current pod index (with wrap-around - eg will not go out of bounds)
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
            bool isActive = i == currentPodIndex;
            podContents[i].SetActive(i == currentPodIndex);
            podPlaceholders[i].SetActive(!isActive);
        }
    }

     // Methods to next/prev btn handle hover events
    public void OnButtonHoverEnter()
    {
        podContents[currentPodIndex].SetActive(false);
        for (int i = 0; i < podPlaceholders.Length; i++)
        {
            podPlaceholders[i].SetActive(true);
        }
    }

    public void OnButtonHoverExit()
    {
        podContents[currentPodIndex].SetActive(true);
        for (int i = 0; i < podPlaceholders.Length; i++)
        {
            podPlaceholders[i].SetActive(i != currentPodIndex);
        }
    }
}
