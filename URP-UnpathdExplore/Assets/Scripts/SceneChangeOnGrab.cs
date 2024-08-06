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
    private string tag10 = "UnpathDesign";

    

    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelectEnter);
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        GameObject item = args.interactable.gameObject;
        ChangeScene(item.tag);
    }

    private void ChangeScene(string itemTag)
    {
        // Stop the current audio
        VoiceoverManager.Stop();

        // Load the new scene
        switch (itemTag)
        {
            case "DGNapoleonic":
                SceneManager.LoadScene("Dumfries and Galloway Napoleonic Voyage");
                break;
            case "LandscapesMesolithic":
                SceneManager.LoadScene("Submerged Landscaped Mesolithic Voyage");
                break;
            case "UnpathDesign":
                SceneManager.LoadScene("Co-Design Voyage");
                break;
            case "WomenShipping":
                SceneManager.LoadScene("Women and Shipping in the 20th Century");
                break;
            case "ResetRefine":
                SceneManager.LoadScene("Database Search");
                break;
            case "RefineOrVoyage":
                SceneManager.LoadScene("RefineOrVoyage");
                break;
            case "Demo":
                SceneManager.LoadScene("Demo");
                break;
            case "Credits":
                SceneManager.LoadScene("Credits");
                break;
            case "SoundScapes":
                SceneManager.LoadScene("SoundScapes");
                break;
            default:
                Debug.Log("No matching tag found.");
                break;
        }
    }

}
