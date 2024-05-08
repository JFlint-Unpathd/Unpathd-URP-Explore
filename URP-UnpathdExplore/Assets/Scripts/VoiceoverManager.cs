using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VoiceoverManager : MonoBehaviour
{
    public static VoiceoverManager instance;
    public GameObject UnpathText;
    public GameObject settingsMenu;
    
    public AudioClip introClip;
    public AudioClip settingsClip;
    public AudioClip explanatoryClip;
    public AudioClip resultsClip;
    
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
        StartCoroutine(IntroAudio(3f, 2f));
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
            StartCoroutine(IntroAudio(3f, 2f));
        }
        if (scene.name == "DatabaseSearch")
        {
            PlayExplanatoryAudio();
        }
    }


    private IEnumerator IntroAudio(float introDelay, float settingsDelay)
    {
        //wait for volume sliders to be populated
        
        yield return null;

        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
        
        yield return new WaitForSecondsRealtime(introDelay);
        AudioManager.instance.PlayClip(introClip);

        yield return new WaitUntil(() => !AudioManager.instance.IsPlaying());

        if (settingsMenu != null)
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

            yield return new WaitForSecondsRealtime(settingsDelay);

            AudioManager.instance.PlayClip(settingsClip);

        }
        else
        {
            Debug.Log("No GameObject assigned to 'settingsMenu'");
        }
    }
    public void PlayExplanatoryAudio()
    {
        StartCoroutine(ExplanatoryAudioDelay(explanatoryClip, 2f));
    }

    private IEnumerator ExplanatoryAudioDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        AudioManager.instance.PlayClip(clip);
    }

        
    public void PlayResultsAudio()
    {
        StartCoroutine(ResultsAudioDelay(resultsClip, 2f));
    }

    private IEnumerator ResultsAudioDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        AudioManager.instance.PlayClip(clip);
    }

}
