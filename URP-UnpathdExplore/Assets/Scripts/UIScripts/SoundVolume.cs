using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundVolume : MonoBehaviour
{
    public List<Slider> volumeSliders = new List<Slider>();
    public List<Slider> onOffSliders = new List<Slider>();

    private AudioSource audioSource;
    public AudioManager audioManager;
    

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        volumeSliders.Clear();
        onOffSliders.Clear();

        FindVolumeSliders();
        FindOnOffSliders();
    }

    
    void Start()
    {
        audioManager = GetComponent<AudioManager>();
        audioSource = GetComponent<AudioSource>();

        volumeSliders.Clear();
        onOffSliders.Clear();

        FindOnOffSliders();
        FindVolumeSliders();

        // Set the on/off sliders to the "on" state
        foreach (Slider slider in onOffSliders)
        {
            slider.value = 1f; // Set to the maximum value, assuming 1 represents the "on" state
        }

        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }

        else
        {
            Load();
        }
    }


    // private void FindOnOffSliders()
    // {
    //     // Search for all on/off sliders by tag
    //     GameObject[] sliderObjects = GameObject.FindGameObjectsWithTag("SoundOnOffSlider");

    //     // Loop through all found GameObjects
    //     foreach (GameObject sliderObject in sliderObjects)
    //     {
    //         Slider slider = sliderObject.GetComponent<Slider>();
    //         if (slider != null)
    //         {
    //             // Add the found slider to the list
    //             onOffSliders.Add(slider);
    //         }
    //     }

    //     // If no on/off sliders are found
    //     if ( onOffSliders.Count == 0)
    //     {
    //         Debug.LogWarning("No on/off sliders found in the scene.");
    //     }
    // }


    private void FindOnOffSliders()
    {
        // Search for all on/off sliders, including inactive ones
        Slider[] sliderObjects = Resources.FindObjectsOfTypeAll<Slider>();

        // Loop through all found GameObjects
        foreach (Slider slider in sliderObjects)
        {
            if (slider.gameObject.CompareTag("SoundOnOffSlider"))
            {
                // Add the found slider to the list
                onOffSliders.Add(slider);
            }
        }
        
        // If no on/off sliders are found
        if ( onOffSliders.Count == 0)
        {
            Debug.LogWarning("No on/off sliders found in the scene.");
        }
    }


    // private void FindVolumeSliders()
    // {
    //     // Search for all volume sliders by tag
    //     GameObject[] sliderObjects = GameObject.FindGameObjectsWithTag("VolumeSlider");
        
    //     // Loop through all found GameObjects
    //     foreach (GameObject sliderObject in sliderObjects)
    //     {
    //         Slider slider = sliderObject.GetComponent<Slider>();
    //         if (slider != null)
    //         {
    //             // Add the found slider to the list
    //             volumeSliders.Add(slider);
    //         }
    //     }

    //     // If no volume sliders are found, you might want to handle this case
    //     if (volumeSliders.Count == 0)
    //     {
    //         Debug.LogWarning("No volume sliders found in the scene.");
    //     }
    // }


    private void FindVolumeSliders()
    {
        // Search for all volume sliders, including inactive ones
        Slider[] sliderObjects = Resources.FindObjectsOfTypeAll<Slider>();

        // Loop through all found GameObjects
        foreach (Slider slider in sliderObjects)
        {
            if (slider.gameObject.CompareTag("VolumeSlider"))
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
            // Check if the slider is currently being interacted with
            if (slider.gameObject == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
            {
                // Set the volume based on the value of the active slider
                AudioListener.volume = slider.value;
                break; // Exit the loop once the active slider is found
            }
        }

        SynchronizeVolumeSliders();
        Save();
    }

    private void SynchronizeVolumeSliders()
    {
        // Check if there are multiple volume sliders
        if (volumeSliders.Count > 1)
        {
            // Get the value of the active volume slider
            float activeVolume = AudioListener.volume;

            // Update the value of all other volume sliders
            foreach (Slider slider in volumeSliders)
            {
                if (slider.gameObject != UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
                {
                    slider.value = activeVolume;
                }
            }
        }
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
            Debug.Log("making it 1");
            // If there are no sliders in the list, default to full volume
            volumeToSave = 1f;
        }

        // Save the volume value to PlayerPrefs
        PlayerPrefs.SetFloat("musicVolume", volumeToSave);
    }


    public void PlayPauseSoundEffects()
    {
        if (AudioListener.volume > 0)
        {
            AudioListener.volume = 0;
            // If audio is muted, set color to red
            SetSlidersColor(Color.red);
        }
        else
        {
            AudioListener.volume = 1;
            // If audio is unmuted, set color to green
            SetSlidersColor(Color.green);
        }

        Save();
    }
    
    private void SetSlidersColor(Color color)
    {
        foreach (Slider slider in onOffSliders)
        {
            // Get the VolumeHandleColorChange component from the handle of the slider
            var handleColorChangeComponent = slider.handleRect.GetComponent<VolumeHandleColorChange>();
            if (handleColorChangeComponent != null)
            {
                // Set the color
                handleColorChangeComponent.SetColor(color);
            }
        }
    }



}
