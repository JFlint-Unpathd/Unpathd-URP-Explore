using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneChangeOnGrab : MonoBehaviour
{
    private string tag1 = "DGNapoleonic";
    private string tag2 = "LandscapesMesolithic";
    private string tag3 = "UnpathDesign";
    private string tag4 = "WomenShipping";
    private string tag5 = "ResetRefine";
    private string tag6 = "RefineOrVoyage";
    private string tag7 = "Demo";
    private string tag8 = "Credits";
    private string tag9 = "SoundScapes";

    

    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelectEnter);
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("Select Entered. Stopping current audio...");
        VoiceoverManager.Stop();

        GameObject item = args.interactable.gameObject;
        if (item.tag == tag1)
        {
            SceneManager.LoadScene("Dumfries and Galloway Napoleonic Voyage");
            VoiceoverManager.instance.HandleSceneAudio("Dumfries and Galloway Napoleonic Voyage");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag2)
        {
            SceneManager.LoadScene("Submerged Landscaped Mesolithic Voyage");
            VoiceoverManager.instance.HandleSceneAudio("Submerged Landscaped Mesolithic Voyage");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag3)
        {
            SceneManager.LoadScene("Co-Design Voyage");
            VoiceoverManager.instance.HandleSceneAudio("Co-Design Voyage");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag4)
        {
            SceneManager.LoadScene("Women and Shipping in the 20th Century");
            VoiceoverManager.instance.HandleSceneAudio("Women and Shipping in the 20th Century");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        
        else if (item.tag == tag5)
        {
            SceneManager.LoadScene("Database Search");
            VoiceoverManager.instance.HandleSceneAudio("Database Search");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag6)
        {
            SceneManager.LoadScene("RefineOrVoyage");
            VoiceoverManager.instance.HandleSceneAudio("RefineOrVoyage");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag7)
        {
            SceneManager.LoadScene("Demo");
            VoiceoverManager.instance.HandleSceneAudio("Demo");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag8)
        {
            SceneManager.LoadScene("Credits");
            VoiceoverManager.instance.HandleSceneAudio("Credits");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag9)
        {
            SceneManager.LoadScene("SoundScapes");
            VoiceoverManager.instance.HandleSceneAudio("SoundScapes");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
   
        else
        {
            Debug.Log("No matching tag found.");
        }
    }

}
