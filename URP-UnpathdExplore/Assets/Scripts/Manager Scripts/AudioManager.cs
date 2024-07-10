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
    //playback time for journey pod VO's
    private float lastPlaybackTime = 0;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; 
        }
    }

    public void PlayClip(AudioClip clip, float startTime = 0)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.time = startTime;
            audioSource.Play();
        }
        else
        {
            Debug.Log("Audio is currently playing. New audio clip will not be played until the current one finishes.");
        }
    }

    public static void Stop() 
    {
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.Stop();
        }
    }   

    //stop method that regards where yo resume clips 
    public void StopAndTrack(int podIndex)
    {
        if (podIndex < 0 || podIndex >= TeleportationManager._instance.lastPlaybackTimes.Length)
        {
            Debug.LogWarning("StopAndTrack: podIndex is out of bounds");
            return;
        }

        if (audioSource.isPlaying)
        {
            TeleportationManager._instance.lastPlaybackTimes[podIndex] = audioSource.time;
            audioSource.Stop();
        }
    }

    // added for MDes Sound Scene
    public void StopClip(AudioClip clip)
    {
        if (audioSource.isPlaying && audioSource.clip == clip)
        {
            audioSource.Stop();
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

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
        else
        {
            Debug.LogError("AudioSource not found.");
        }
    }

    public float GetLastPlaybackTime()
    {
        return lastPlaybackTime;
    }

    public void ResumeClip(float lastPlaybackTime)
    {
        PlayClip(audioSource.clip, lastPlaybackTime);
    }
}
