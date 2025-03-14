using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelayManager : MonoBehaviour
{
    public AudioSource m_AudioSource;

    private static AudioDelayManager _Instance;

    private void Awake()
    {
        if(_Instance != null)
        {
            Debug.LogWarning("TRYING TO CREATE TWO AUDIO DELAY MANAGERS!");
            return;
        }
        _Instance = this;
    }

    public static void SetClip(AudioClip clip, float delay)
    {
        _Instance.m_AudioSource.Stop();
        _Instance.m_AudioSource.clip = clip;
        _Instance.m_AudioSource.PlayDelayed(delay);
    }

    public static void Stop()
    {
        _Instance.m_AudioSource.Stop();
    }
}
