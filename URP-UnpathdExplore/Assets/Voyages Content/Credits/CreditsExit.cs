using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CreditsExit : MonoBehaviour
{
public VideoPlayer videoPlayer; // Assign the VideoPlayer component in the Inspector
    public GameObject exitItem;
    public float delay = 5f; // Delay in seconds

    private void Start()
    {
        // Subscribe to the video player's loopPointReached event
        videoPlayer.loopPointReached += OnVideoEnd;
        
        // Ensure the exitItem is initially inactive
        exitItem.SetActive(false);
        
        // Start playing the video
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Show the exitItem when the video ends
        exitItem.SetActive(true);
        
        // Start the coroutine to wait for 5 seconds and then hide the exitItem
        StartCoroutine(HideexitItemAfterDelay());
    }

    private IEnumerator HideexitItemAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        // Hide the exitItem
        exitItem.SetActive(false);
        
        // Restart the video
        videoPlayer.Play();
    }
}
    
