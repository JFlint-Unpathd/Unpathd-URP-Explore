// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class AudioDelayManager : MonoBehaviour
// {
//     public static AudioDelayManager instance;


//     private void Awake()
//     {
//         // Singleton pattern to ensure there's only one instance of this manager
//         if (instance == null)
//         {
//             instance = this;
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     void Start()
//     {
        
//     }

//     public void StartExplanatoryAudioDelay(AudioClip audioClip, float delay)
//     {
//         StartCoroutine(ExplanatoryAudioDelay(audioClip, delay));
//     }

//     private IEnumerator ExplanatoryAudioDelay(AudioClip audioClip, float delay)
//     {
//         yield return new WaitForSecondsRealtime(delay);
//         AudioManager.instance.PlayClip(audioClip);
//         Debug.Log("Coroutine started for audio delay");
//     }
// }
