using UnityEngine;
using System.Collections;

public class IntroVoiceover : MonoBehaviour
{
    public GameObject UnpathText;
    public GameObject settingsMenu;
    
    public AudioClip introClip;
    public AudioClip settingsClip;
    public AudioClip explanatoryClip;
    public AudioClip resultsClip;
    
    private AudioSource audioSource;

    void Start()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }

        if(AudioManager.instance == null)
        {
            Debug.LogError("No AudioManager found in the scene. Please make sure you have the AudioManager in the scene and it is enabled.");
            return;
        }

    
        StartCoroutine(PlayClipsWithDelay(3f, 2f));
    }

    private IEnumerator PlayClipsWithDelay(float introDelay, float settingsDelay)
    {
        yield return new WaitForSecondsRealtime(introDelay);
        AudioManager.instance.PlayClip(introClip);

        yield return new WaitUntil(() => !AudioManager.instance.IsPlaying());

        if (settingsMenu != null)
        {
            if (UnpathText != null)
            {
                UnpathText.SetActive(false);
            }
            
            settingsMenu.SetActive(true);

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
