using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelay : MonoBehaviour
{
    public AudioClip imgSettingsVo;
  
    public void StartCor()
    {
        StartCoroutine(ExplanatoryAudioDelay(imgSettingsVo, 2f));
    }

    private IEnumerator ExplanatoryAudioDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        AudioManager.instance.PlayClip(imgSettingsVo);
    }

    
}
