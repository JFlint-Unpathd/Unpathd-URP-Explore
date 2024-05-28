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


    [Header("Voyage Start VOs")]
    public AudioClip[] napoleonicClips;
    public AudioClip[] mesolithicClips;
    public AudioClip[] coDesignClips;
    public AudioClip[] shippingClips;

    private AudioSource audioSource;

    private bool introClipsFinished = false;
     private bool settingsAudioPlayed = false;

    void Awake()
    {
        // Ensure only one instance of SceneAudioManager exists
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

    }

    void Start()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(SettingsMenuDisable());
        StartCoroutine(PlayAudioClipsSequentially(introClips, 3f));
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if this is the IntroScene
        if (scene.name == "IntroMenu")
        {
            Debug.Log("detected");
            StartCoroutine(PlayAudioClipsSequentially(introClips, 3f));
        }
        if (scene.name == "DatabaseSearch")
        {
            PlaySearchRoomAudio();
        }
    }

    private IEnumerator SettingsMenuDisable()
    {
        yield return null;
        settingsMenu.SetActive(false);
    }

    private IEnumerator PlayAudioClipsSequentially(AudioClip[] clips, float initialDelay)
    {
        // Initial delay
        yield return new WaitForSecondsRealtime(initialDelay);

        foreach (AudioClip clip in clips)
        {
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
            
                StartCoroutine(PlayAudioClipsSequentially(settingsClips, 1f));
                settingsAudioPlayed = true;
            }

        }
        else if (clips == null)
        {
            Debug.Log("No GameObject assigned to 'settingsMenu'");
        }
    }

    public void PlaySearchRoomAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(searchRoomClips, 2f));
    }

    private IEnumerator ExplanatoryAudioDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        AudioManager.instance.PlayClip(clip);
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


    public void HandleSceneAudio(string sceneName)
    {
        if (sceneName == "Dumfries and Galloway Napoleonic Voyage")
        {
            NapoleonicVoyageAudio();
            
        }
        else if (sceneName == "Submerged Landscaped Mesolithic Voyage")
        {
            MesolithicVoyageAudio();
            
        }
        else if (sceneName == "Co-Design Voyage")
        {
            CoDesignVoyageAudio();
            
        }
        else if (sceneName == "Women and Shipping in the 20th Century")
        {
            WomenShippingVoyageAudio();
            
        }
        else if (sceneName == "Database Search")
        {
            PlaySearchRoomAudio();
            
        }
        else if (sceneName == "RefineOrVoyage")
        {
            PlayRefineorVoyageAudio();    
        }
        else if (sceneName == "Demo")
        {
            DemoAudio();
            
        }
        else
        {
            Debug.Log("No matching tag found.");
        }

    }

}
