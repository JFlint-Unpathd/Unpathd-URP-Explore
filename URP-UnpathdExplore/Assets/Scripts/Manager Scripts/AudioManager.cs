using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    private AudioSource audioSource;

    // Expose audioSource property
    public AudioSource GetAudioSource()
    {
        return audioSource;
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            PersistanceClass.DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; 
        }
    }

    public void PlayClip(AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        
        else
        {
            Debug.Log("Audio is currently playing. New audio clip will not be played until the current one finishes.");
        }
    }

    public static void Stop() 
    {
        //instance.audioSource.Stop();
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.Stop();
        }
    }   

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

        public void StopAll()
    {
        audioSource.Stop();
    }

    public bool IsPlaying(AudioClip clip)
    {
        return audioSource.isPlaying && audioSource.clip == clip;
    }
}
