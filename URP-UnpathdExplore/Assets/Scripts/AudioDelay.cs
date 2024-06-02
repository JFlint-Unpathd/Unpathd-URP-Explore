using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelay : MonoBehaviour
{
    public AudioClip imgSettingsVo;

    public void StartAudioDelay()
    {
        //AudioDelayManager.instance.StartExplanatoryAudioDelay(imgSettingsVo, 1f);
        Debug.Log("Audio delay started");
    }
}
