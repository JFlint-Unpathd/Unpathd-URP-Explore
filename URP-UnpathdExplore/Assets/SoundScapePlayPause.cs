using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SoundScapePlayPause : MonoBehaviour
{
    public AudioClip voiceoverClip;
    
    private XRBaseInteractable interactable;
    private ColorInteractionChange colorChanger;
    private bool isPlaying = false;

    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);

        // Access the ColorInteractionChange component
        colorChanger = GetComponentInChildren<ColorInteractionChange>();
        colorChanger.SetSoundScapeInstance(true); // Mark this instance as a SoundScape instance
    }

    private void Update()
    {
        bool currentlyPlaying = AudioManager.instance.IsPlaying(voiceoverClip);

        if (currentlyPlaying && !isPlaying)
        {
            // Audio started playing
            colorChanger.SetPlayingColor(colorChanger.colorProperties.snappedColor);
            isPlaying = true;
        }
        else if (!currentlyPlaying && isPlaying)
        {
            // Audio stopped playing
            colorChanger.ResetColor();
            isPlaying = false;
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Stop audio on all objects except the selected one
        SoundScapePlayPause[] allSoundScapes = FindObjectsOfType<SoundScapePlayPause>();
        foreach (var soundScape in allSoundScapes)
        {
            if (soundScape != this)
            {
                soundScape.StopAudio();
            }
        }

        // Play the current audio clip
        AudioManager.instance.PlayClip(voiceoverClip);
    }

    private void StopAudio()
    {
        // Stop audio playback on this object
        AudioManager.instance.StopClip(voiceoverClip);
    }
}
