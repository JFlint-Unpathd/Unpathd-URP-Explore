using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VoiceoverManager : MonoBehaviour
{
    public static VoiceoverManager instance;
    public GameObject UnpathText;
    public GameObject settingsMenu;
    
    [Header("Scene Start VOs")]
    public AudioClip[] introClips;
    public bool[] introClipsDescriptive; // Add a boolean array to mark descriptive clips
    public AudioClip[] settingsClips;
    public bool[] settingsClipsDescriptive;
    public AudioClip[] searchRoomClips;
    public bool[] searchRoomClipsDescriptive;
    public AudioClip[] resultsClips;
    public bool[] resultsClipsDescriptive;
    public AudioClip[] refineOrVoyageClips;
    public bool[] refineOrVoyageClipsDescriptive;
    public AudioClip[] demoAudioClips;
    public bool[] demoAudioClipsDescriptive;
    public AudioClip[] creditsAudioClips;
    public bool[] creditsAudioClipsDescriptive;

    [Header("Voyage Start VOs")]
    public AudioClip[] napoleonicClips;
    public bool[] napoleonicClipsDescriptive;
    public AudioClip[] mesolithicClips;
    public bool[] mesolithicClipsDescriptive;
    public AudioClip[] coDesignClips;
    public bool[] coDesignClipsDescriptive;
    public AudioClip[] shippingClips;
    public bool[] shippingClipsDescriptive;

    private AudioSource audioSource;

    private bool introClipsFinished = false;
    private bool settingsAudioPlayed = false;
    public bool demoAudioPlayed = false;

    private bool refineOrVoyagePlayed = false;
    public bool stopAudio;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            PersistanceClass.DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(SettingsMenuDisable());
        StartCoroutine(PlayAudioClipsSequentially(introClips, introClipsDescriptive, 2f));
    }
    
    public static void Stop() 
    {
        Debug.Log("VoiceoverManager: Stopping current audio...");
        AudioManager.Stop();
        if (instance != null)
        {
            instance.stopAudio = true;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Stop();

        Debug.Log("Scene Loaded: " + scene.name);
        if (scene.name == "IntroMenu")
        {
            Debug.Log("detected");
            StartCoroutine(PlayAudioClipsSequentially(introClips, introClipsDescriptive, 2f));
        }
        if (scene.name == "DatabaseSearch")
        {
            PlaySearchRoomAudio();
        }
        
        HandleSceneAudio(scene.name);
    }

    private IEnumerator SettingsMenuDisable()
    {
        yield return null;
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
    }

    private IEnumerator PlayAudioClipsSequentially(AudioClip[] clips, bool[] isDescriptive, float initialDelay)
    {
        yield return new WaitForSecondsRealtime(initialDelay);

        for (int i = 0; i < clips.Length; i++)
        {
            if (stopAudio)
            {
                stopAudio = false;
                Debug.Log("Audio sequence stopped.");
                yield break;
            }

            // Check if the clip is descriptive and if it should be muted
            if (isDescriptive[i] && SoundVolume.instance.IsMuted())
            {
                continue;
            }

            AudioManager.instance.PlayClip(clips[i]);
            yield return new WaitUntil(() => !AudioManager.instance.IsPlaying());
        }

        if (clips == introClips)
        {
            introClipsFinished = true;
        }

        if (introClipsFinished && settingsMenu != null)
        {
            if (UnpathText != null)
            {
                UnpathText.SetActive(false);
            }

            foreach (Transform child in settingsMenu.transform)
            {
                child.gameObject.SetActive(false);
            }

            settingsMenu.SetActive(true);

            if (settingsMenu.transform.childCount > 0)
            {
                Transform firstChild = settingsMenu.transform.GetChild(0);
                firstChild.gameObject.SetActive(true);
            }

            yield return new WaitForSecondsRealtime(initialDelay);

            if (!settingsAudioPlayed)
            {
                StartCoroutine(PlaySettingsAudioSequentially(settingsClips, settingsClipsDescriptive));
                settingsAudioPlayed = true;
            }
        }
        else if (clips == null)
        {
            Debug.Log("No GameObject assigned to 'settingsMenu'");
        }
    }

    private IEnumerator PlaySettingsAudioSequentially(AudioClip[] clips, bool[] isDescriptive)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            if (isDescriptive[i] && SoundVolume.instance.IsMuted())
            {
                continue;
            }

            AudioManager.instance.PlayClip(clips[i]);
            yield return new WaitUntil(() => !AudioManager.instance.IsPlaying());
        }
    }

    public void PlaySearchRoomAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(searchRoomClips, searchRoomClipsDescriptive, 2f));
    }
        
    public void PlayResultsAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(resultsClips, resultsClipsDescriptive, 2f));
    }

    public IEnumerator PlayRefineorVoyageAudio()
    {
        if (!refineOrVoyagePlayed)
        {
            refineOrVoyagePlayed = true;
            yield return StartCoroutine(PlayAudioClipsSequentially(refineOrVoyageClips, refineOrVoyageClipsDescriptive, 2f));
        }
    }

    public void NapoleonicVoyageAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(napoleonicClips, napoleonicClipsDescriptive, 2f));
    }

    public void MesolithicVoyageAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(mesolithicClips, mesolithicClipsDescriptive, 2f));
    }

    public void CoDesignVoyageAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(coDesignClips, coDesignClipsDescriptive, 2f));
    }

    public void WomenShippingVoyageAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(shippingClips, shippingClipsDescriptive, 2f));
    }

    public void DemoAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(demoAudioClips, demoAudioClipsDescriptive, 2f));
    }

    public void CreditsAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(creditsAudioClips, creditsAudioClipsDescriptive, 2f));
    }

    public void HandleSceneAudio(string sceneName)
    {
        switch (sceneName)
        {
            case "Dumfries and Galloway Napoleonic Voyage":
                NapoleonicVoyageAudio();
                break;
            case "Submerged Landscaped Mesolithic Voyage":
                MesolithicVoyageAudio();
                break;
            case "Co-Design Voyage":
                CoDesignVoyageAudio();
                break;
            case "Women and Shipping in the 20th Century":
                WomenShippingVoyageAudio();
                break;
            case "Database Search":
                PlaySearchRoomAudio();
                break;
            case "RefineOrVoyage":
                StartCoroutine(PlayRefineorVoyageAudio());
                break;
            case "Demo":
                DemoAudio();
                break;
            case "Credits":
                CreditsAudio();
                break;
            default:
                Debug.Log("No matching tag found.");
                break;
        }
    }
}
