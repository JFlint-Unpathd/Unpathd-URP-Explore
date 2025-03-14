using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelay : MonoBehaviour
{
    public AudioClip m_Clip;
    public float m_DelaySeconds;

    public void OnEnable()
    {
        if (m_Clip != null)
        {
            AudioDelayManager.SetClip(m_Clip, m_DelaySeconds);
        }
    }

    private void OnDisable()
    {
        AudioDelayManager.Stop();
    }
}
