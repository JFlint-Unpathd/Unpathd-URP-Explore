using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolume : MonoBehaviour
{
    [SerializeField] List<Slider> volumeSliders = new List<Slider>();

    private AudioSource audioSource;
    

    void Start()
    {
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }

        else
        {
            Load();
        }

        audioSource = GetComponent<AudioSource>();

        // Find the volume slider dynamically
        FindVolumeSliders();
        
    }

    private void FindVolumeSliders()
    {
        // Search for all volume sliders by tag
        GameObject[] sliderObjects = GameObject.FindGameObjectsWithTag("VolumeSlider");

        // Loop through all found GameObjects
        foreach (GameObject sliderObject in sliderObjects)
        {
            Slider slider = sliderObject.GetComponent<Slider>();
            if (slider != null)
            {
                // Add the found slider to the list
                volumeSliders.Add(slider);
            }
        }

        // If no volume sliders are found, you might want to handle this case
        if (volumeSliders.Count == 0)
        {
            Debug.LogWarning("No volume sliders found in the scene.");
        }
    }

    public void ChangeVolume()
    {

        foreach (Slider slider in volumeSliders)
        {
            // Set the volume for each found slider
            AudioListener.volume = slider.value;
        }

        Save();

    }

    private void Load()
    {
        float savedVolume = PlayerPrefs.GetFloat("musicVolume");

        // Set the saved volume to all volume sliders
        foreach (Slider slider in volumeSliders)
        {
            slider.value = savedVolume;
        }
    }



    private void Save()
    {
        float volumeToSave;

        if (volumeSliders.Count > 0)
        {
            // If there's at least one slider in the list, get the volume value from the first slider
            volumeToSave = volumeSliders[0].value;
        }
        else
        {
            // If there are no sliders in the list, default to full volume
            volumeToSave = 1f;
        }

        // Save the volume value to PlayerPrefs
        PlayerPrefs.SetFloat("musicVolume", volumeToSave);
    }



    public void PlayPauseSoundEffects()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }
}
