using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    public TeleportationProvider teleportationProvider;
    public GameObject[] teleportationPods;
    public GameObject[] podContents;
    public GameObject[] podPlaceholders;
    public AudioClip[] podVoiceovers;

    public float[] lastPlaybackTimes; // Stores the last playback times for each pod
    private int currentPodIndex = -1; // Tracks the current pod index (-1 means no pod selected)

    private bool voyageStarted = false;

    public static TeleportationManager _instance;

    public static void SetPodIndex(int index)
    {
        if (index < 0 || index >= _instance.podContents.Length)
        {
            Debug.LogWarning("SetPodIndex: index is out of bounds");
            return;
        }

        _instance.TeleportToPod(index); // Use TeleportToPod to handle teleportation and audio

    }

    private void Awake()
    {
        _instance = this;
        EnsureAudioSources();
    }

    void Start()
    {
        // Initialize lastPlaybackTimes array
        lastPlaybackTimes = new float[podVoiceovers.Length];

        // Initially hide all pod contents
        for (int i = 0; i < podContents.Length; i++)
        {
            podContents[i].SetActive(false);
        }

        // Show the placeholder for the first pod initially, if available
        if (podPlaceholders.Length > 0)
        {
            podPlaceholders[0].SetActive(true);
        }
    }

    public void TeleportToNextPod()
    {
        if (currentPodIndex == -1)
        {
            TeleportToPod(0);
            return;
        }

        int nextIndex = (currentPodIndex + 1) % teleportationPods.Length;
        TeleportToPod(nextIndex);
    }

    public void TeleportToPreviousPod()
    {
        if (currentPodIndex == -1)
        {
            Debug.LogWarning("TeleportToPreviousPod: No current pod to teleport from");
            return;
        }

        int newIndex = (currentPodIndex - 1 + teleportationPods.Length) % teleportationPods.Length;
        TeleportToPod(newIndex);
    }

    // Handles the teleportation logic to a specific pod
    private void TeleportToPod(int podIndex)
    {
        if (podIndex < 0 || podIndex >= teleportationPods.Length)
        {
            Debug.LogWarning("TeleportToPod: podIndex is out of bounds");
            return;
        }

        // Stop current audio and track last playback time
        if (currentPodIndex != -1) 
        {
            AudioManager.instance.StopAndTrack(currentPodIndex);
        }

        currentPodIndex = podIndex;

        TeleportationAnchor currentPodTeleportationAnchor = teleportationPods[currentPodIndex].GetComponent<TeleportationAnchor>();
        if (currentPodTeleportationAnchor != null)
        {
            TeleportRequest teleportRequest = new TeleportRequest
            {
                destinationPosition = currentPodTeleportationAnchor.teleportAnchorTransform.position,
                destinationRotation = currentPodTeleportationAnchor.teleportAnchorTransform.rotation
            };
            teleportationProvider.QueueTeleportRequest(teleportRequest);
        }

        voyageStarted = true;
        PlayPodVoiceover(currentPodIndex);
        UpdatePodContentsVisibility();
    }

    private void PlayPodVoiceover(int podIndex)
    {
        if (podIndex < 0 || podIndex >= podVoiceovers.Length)
        {
            Debug.LogWarning("PlayPodVoiceover: podIndex is out of bounds");
            return;
        }

        AudioClip clip = podVoiceovers[podIndex];
        float startTime = lastPlaybackTimes[podIndex]; // Get the last playback time for the pod

        if (clip != null)
        {
            AudioManager.instance.PlayClip(clip, startTime); // Start playing from the last playback time
        }
    }


    private void UpdatePodContentsVisibility()
    {
        for (int i = 0; i < podContents.Length; i++)
        {
            bool isActive = i == currentPodIndex;
            podContents[i].SetActive(isActive);
            podPlaceholders[i].SetActive(!isActive);
        }
    }

    private void EnsureAudioSources()
    {
        foreach (GameObject pod in teleportationPods)
        {
            if (pod.GetComponent<AudioSource>() == null)
            {
                pod.AddComponent<AudioSource>(); // Add an AudioSource if it's missing
            }
        }
        Debug.Log("All pods are ensured to have AudioSources.");
    }

    public void OnButtonHoverEnter()
    {
        if (currentPodIndex < 0 || currentPodIndex >= podContents.Length)
        {
            Debug.LogWarning("OnButtonHoverEnter: currentPodIndex is out of bounds");
            return;
        }

        podContents[currentPodIndex].SetActive(false);
        for (int i = 0; i < podPlaceholders.Length; i++)
        {
            if (i != currentPodIndex)
            {
                podPlaceholders[i].SetActive(true);
            }
        }
    }

    public void OnButtonHoverExit()
    {
        if (currentPodIndex < 0 || currentPodIndex >= podContents.Length)
        {
            Debug.LogWarning("OnButtonHoverExit: currentPodIndex is out of bounds");
            return;
        }

        podContents[currentPodIndex].SetActive(true);
        for (int i = 0; i < podPlaceholders.Length; i++)
        {
            if (i != currentPodIndex)
            {
                podPlaceholders[i].SetActive(false);
            }
        }
    }
}

