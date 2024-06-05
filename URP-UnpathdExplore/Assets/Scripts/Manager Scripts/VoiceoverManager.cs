using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VoiceoverManager : MonoBehaviour
{
    public static VoiceoverManager instance;
    public GameObject UnpathText;
    public GameObject settingsMenu;
    
    [Header("Scene Start VOs")]
    public AudioClip[] introClips;
    public AudioClip[] settingsClips;
    public AudioClip[] searchRoomClips;
    public AudioClip[] resultsClips;
    public AudioClip[] refineOrVoyageClips;
    public AudioClip[] demoAudioClips;
    public AudioClip[] creditsAudioClips;

    [Header("Voyage Start VOs")]
    public AudioClip[] napoleonicClips;
    public AudioClip[] mesolithicClips;
    public AudioClip[] coDesignClips;
    public AudioClip[] shippingClips;

    private AudioSource audioSource;

    private bool introClipsFinished = false;
    private bool settingsAudioPlayed = false;
    public bool demoAudioPlayed = false;
    public bool stopAudio;

    void Awake()
    {
        // Ensure only one instance of VoiceoverManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance exists, destroy this one
            Destroy(gameObject);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(SettingsMenuDisable());
        StartCoroutine(PlayAudioClipsSequentially(introClips, 2f));
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
        //Stop any audio playing from the previous scene
        Stop();

        Debug.Log("Scene Loaded: " + scene.name);
        // Check if this is the IntroScene
        if (scene.name == "IntroMenu")
        {
            Debug.Log("detected");
            StartCoroutine(PlayAudioClipsSequentially(introClips, 2f));
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

    private IEnumerator PlayAudioClipsSequentially(AudioClip[] clips, float initialDelay)
    {
        // Initial delay
        yield return new WaitForSecondsRealtime(initialDelay);

        foreach (AudioClip clip in clips)
        {
            if (stopAudio)
            {
                stopAudio = false;
                Debug.Log("Audio sequence stopped.");
                yield break;
            }
            AudioManager.instance.PlayClip(clip);
            yield return new WaitUntil(() => !AudioManager.instance.IsPlaying());
        }

        // Check if these are the intro clips
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

            // Deactivate all children first
            foreach (Transform child in settingsMenu.transform)
            {
                child.gameObject.SetActive(false);
            }

            settingsMenu.SetActive(true);

            // Then activate only the first child
            if (settingsMenu.transform.childCount > 0)
            {
                Transform firstChild = settingsMenu.transform.GetChild(0);
                firstChild.gameObject.SetActive(true);
            }

            yield return new WaitForSecondsRealtime(initialDelay);

            if (!settingsAudioPlayed)
            {
                StartCoroutine(PlaySettingsAudioSequentially(settingsClips));
                settingsAudioPlayed = true;
            }
        }
        else if (clips == null)
        {
            Debug.Log("No GameObject assigned to 'settingsMenu'");
        }
    }

    private IEnumerator PlaySettingsAudioSequentially(AudioClip[] clips)
    {
        foreach (AudioClip clip in clips)
        {
            AudioManager.instance.PlayClip(clip);
            yield return new WaitUntil(() => !AudioManager.instance.IsPlaying());
        }
    }

    public void PlaySearchRoomAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(searchRoomClips, 2f));
    }
        
    public void PlayResultsAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(resultsClips, 2f));
    }

    public void PlayRefineorVoyageAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(refineOrVoyageClips, 2f));
    }

    public void NapoleonicVoyageAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(napoleonicClips, 2f));
    }

    public void MesolithicVoyageAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(mesolithicClips, 2f));
    }

    public void CoDesignVoyageAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(coDesignClips, 2f));
    }

    public void WomenShippingVoyageAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(shippingClips, 2f));
    }

    public void DemoAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(demoAudioClips, 2f));
        
    }

    public void CreditsAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(creditsAudioClips, 2f));
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
                PlayRefineorVoyageAudio();
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
