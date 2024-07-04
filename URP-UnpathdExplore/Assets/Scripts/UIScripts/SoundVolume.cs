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
            slider.onValueChanged.AddListener(SetVolume);
        }

        foreach (Slider slider in onOffSliders)
        {
            slider.onValueChanged.AddListener(SetVolume);
        }

        foreach (Slider slider in persistentVolumeSliders)
        {
            slider.onValueChanged.AddListener(SetVolume);
        }

        foreach (Slider slider in persistentOnOffSliders)
        {
            slider.onValueChanged.AddListener(SetVolume);
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

    public void SetVolume(float volume)
    {
        foreach (Slider slider in volumeSliders)
        {
            PlayerPrefs.SetFloat("Volume", slider.value);
        }

        foreach (Slider slider in persistentVolumeSliders)
        {
            PlayerPrefs.SetFloat("Volume", slider.value);
        }

        foreach (Slider slider in onOffSliders)
        {
            PlayerPrefs.SetFloat(slider.name, slider.value);
        }

        foreach (Slider slider in persistentOnOffSliders)
        {
            PlayerPrefs.SetFloat(slider.name, slider.value);
        }

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
