using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SoundScapePlayPause : MonoBehaviour
{
    public AudioClip voiceoverClip;
    public Color playingColor = Color.red;
    public Color originalColor = Color.white;

    private XRBaseInteractable interactable;
    private MeshRenderer[] meshRenderers;
    private bool isPlaying = false;

    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);

        // Get all MeshRenderer components in children
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        // Store the original colors
        foreach (var renderer in meshRenderers)
        {
            renderer.material.color = originalColor;
        }
    }

    private void Update()
    {
        // Check if the audio clip is playing and change color accordingly
        if (AudioManager.instance.IsPlaying(voiceoverClip))
        {
            if (!isPlaying)
            {
                ChangeColor(playingColor);
                isPlaying = true;
            }
        }
        else
        {
            if (isPlaying)
            {
                ChangeColor(originalColor);
                isPlaying = false;
            }
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

    private void ChangeColor(Color color)
    {
        foreach (var renderer in meshRenderers)
        {
            renderer.material.color = color;
        }
    }

    private void StopAudio()
    {
        // Stop audio playback on this object
        AudioManager.instance.StopAll();
    }
}
