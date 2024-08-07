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
        if (audioSource.isPlaying)
        {
            Debug.Log("Audio is currently playing. Stopping current audio to play new clip.");
            Stop(); // Ensure the current audio is stopped before playing a new one

        }
        
        Debug.Log("AudioManager: Playing clip " + clip.name);
        audioSource.clip = clip;
        audioSource.time = startTime;
        audioSource.Play();
    }

    public static void Stop() 
    {
        if (instance != null && instance.audioSource != null)
        {
            Debug.Log("AudioManager: Stopping audio clip...");
            instance.audioSource.Stop();
            instance.audioSource.clip = null;
        }
    }   


    public void StopAndTrack(int podIndex)
    {
        if (podIndex < 0 || podIndex >= TeleportationManager._instance.lastPlaybackTimes.Length)
        {
            Debug.LogWarning("StopAndTrack: podIndex is out of bounds");
            return;
        }

        if (instance.audioSource.isPlaying)
        {
            float playbackTime = instance.audioSource.time;
            float clipLength = instance.audioSource.clip.length;
            
            // Check if playback time is near the end of the clip
            if (playbackTime >= (clipLength - 0.1f)) // 0.1 seconds threshold
            {
                TeleportationManager._instance.lastPlaybackTimes[podIndex] = 0; // Reset if finished
            }
            else
            {
                TeleportationManager._instance.lastPlaybackTimes[podIndex] = playbackTime;
            }
            instance.audioSource.Stop();
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
        audioSource.clip = null;
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
