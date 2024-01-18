// 2024-01-18 AI-Tag 
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;
using System.Collections;

public class IntroVoiceover : MonoBehaviour
{
    public AudioClip introClip;
    public AudioClip settingsClip;
    public GameObject settingsMenu;
    private AudioSource audioSource;

    void Start()
    {
        settingsMenu.SetActive(false);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = introClip;
        StartCoroutine(PlayClipsWithDelay(5f, 3f));
    }

    private IEnumerator PlayClipsWithDelay(float introDelay, float settingsDelay)
    {
        yield return new WaitForSecondsRealtime(introDelay);
        audioSource.Play();
        yield return new WaitUntil(() => !audioSource.isPlaying);

        if (settingsMenu != null)
        {
            settingsMenu.SetActive(true);
            yield return new WaitForSecondsRealtime(settingsDelay);
            audioSource.clip = settingsClip;
            audioSource.Play();
        }
        else
        {
            Debug.Log("No GameObject assigned to 'settingsMenu'");
        }
    }
}
