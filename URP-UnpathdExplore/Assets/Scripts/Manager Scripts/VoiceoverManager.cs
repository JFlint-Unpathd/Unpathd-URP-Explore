using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VoiceoverManager : MonoBehaviour
{
    public static VoiceoverManager instance;
    public GameObject UnpathText;
    public GameObject settingsMenu;
    
    public AudioClip[] introClips;
    public AudioClip[] settingsClips;
    public AudioClip[] explanatoryClips;
    public AudioClip[] resultsClips;
    
    private AudioSource audioSource;

    
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
            PlayExplanatoryAudio();
        }
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

        if (clips == introClips && settingsMenu != null)
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

            // Play settings audio clips
            StartCoroutine(PlayAudioClipsSequentially(settingsClips, 2f));
        }
        else if (clips == null)
        {
            Debug.Log("No GameObject assigned to 'settingsMenu'");
        }
    }

    public void PlayExplanatoryAudio()
    {
        StartCoroutine(PlayAudioClipsSequentially(explanatoryClips, 2f));
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


}
