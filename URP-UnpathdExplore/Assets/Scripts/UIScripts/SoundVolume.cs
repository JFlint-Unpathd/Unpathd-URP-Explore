using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class SoundVolume : MonoBehaviour
{
    public static SoundVolume instance;
    [SerializeField]
    private List<Slider> volumeSliders = new List<Slider>();

    [SerializeField]
    private List<Slider> onOffSliders = new List<Slider>();

    [SerializeField]
    private List<Slider> persistentVolumeSliders = new List<Slider>(); 

    [SerializeField]
    private List<Slider> persistentOnOffSliders = new List<Slider>();

    private AudioSource audioSource;
    public AudioManager audioManager;

    public Action<bool> OnMuteStateChanged;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    
    void Start()
    {
        LoadVolumeSettings();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        volumeSliders.Clear();
        onOffSliders.Clear();

        FindVolumeSliders();
        FindOnOffSliders();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        volumeSliders.Clear();
        onOffSliders.Clear();

        volumeSliders.AddRange(persistentVolumeSliders);
        onOffSliders.AddRange(persistentOnOffSliders);
    }

    public bool IsMuted()
    {
        return onOffSliders.Any(slider => slider.value == 0);
    }

    private void LoadVolumeSettings()
    {
        foreach (Slider slider in volumeSliders)
        {
            slider.onValueChanged.AddListener(delegate { ChangeVolume(); });
        }

        foreach (Slider slider in onOffSliders)
        {
            slider.onValueChanged.AddListener(delegate { ChangeVolume(); });
        }

        foreach (Slider slider in persistentVolumeSliders)
        {
            slider.onValueChanged.AddListener(delegate { ChangeVolume(); });
        }

        foreach (Slider slider in persistentOnOffSliders)
        {
            slider.onValueChanged.AddListener(delegate { ChangeVolume(); });
        }

        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        foreach (Slider slider in volumeSliders)
        {
            slider.value = savedVolume;
        }

        foreach (Slider slider in persistentVolumeSliders)
        {
            slider.value = savedVolume;
        }

        foreach (Slider slider in onOffSliders)
        {
            slider.value = PlayerPrefs.GetFloat(slider.name, slider.value);
        }

        foreach (Slider slider in persistentOnOffSliders)
        {
            slider.value = PlayerPrefs.GetFloat(slider.name, slider.value);
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
        SaveVolumeSettings();
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

    private void SaveVolumeSettings()
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
        PlayerPrefs.SetFloat("Volume", volumeToSave);
    }

    public void ToggleMute()
    {
        // Toggle the mute state based on the slider value
        bool isMuted = onOffSliders.Any(slider => slider.value == 0);
        PlayerPrefs.SetInt("DescriptiveMuted", isMuted ? 1 : 0);

        Debug.Log("Descriptive audio muted: " + isMuted);

        // Trigger the mute state change event
        OnMuteStateChanged?.Invoke(isMuted);
    }

    private void FindOnOffSliders()
    {
        // Add persistent sliders that are attached from the Inspector
        onOffSliders.AddRange(persistentOnOffSliders);

        // Search for all on/off sliders, including inactive ones
        Slider[] sliderObjects = Resources.FindObjectsOfTypeAll<Slider>();

        // Loop through all found GameObjects
        foreach (Slider slider in sliderObjects)
        {
            if (slider.gameObject.CompareTag("SoundOnOffSlider") && !persistentOnOffSliders.Contains(slider))
            {
                // Add the found slider to the list
                onOffSliders.Add(slider);
            }
        }
        
        // If no on/off sliders are found
        if (onOffSliders.Count == 0)
        {
            Debug.LogWarning("No on/off sliders found in the scene.");
        }
    }

    private void FindVolumeSliders()
    {
        // Add persistent sliders that are attached from the Inspector
        volumeSliders.AddRange(persistentVolumeSliders);

        // Search for all volume sliders, including inactive ones
        Slider[] sliderObjects = Resources.FindObjectsOfTypeAll<Slider>();

        // Loop through all found GameObjects
        foreach (Slider slider in sliderObjects)
        {
            if (slider.gameObject.CompareTag("VolumeSlider") && !persistentVolumeSliders.Contains(slider))
            {
                // Add the found slider to the list
                volumeSliders.Add(slider);
            }
        }

        // If no volume sliders are found
        if (volumeSliders.Count == 0)
        {
            Debug.LogWarning("No volume sliders found in the scene.");
        }
    }
}
