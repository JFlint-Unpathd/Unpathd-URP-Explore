using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioDescription : MonoBehaviour
{
    public AudioClip voiceoverClip;

    public void PlayDescription()
    {
        AudioManager.instance.PlayClip(voiceoverClip);
    }

}
