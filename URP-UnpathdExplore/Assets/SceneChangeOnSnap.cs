using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneChangeOnSnap : MonoBehaviour
{
    private string tag1 = "DGNapoleonic";
    private string tag2 = "LandscapesMesolithic";
    private string tag3 = "UnpathDesign";
    private string tag4 = "WomenShipping";

    private XRSocketInteractor socketInteractor;

    private void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketInteractor.selectEntered.AddListener(OnSelectEnter);
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        GameObject item = args.interactable.gameObject;
        if (item.tag == tag1)
        {
            SceneManager.LoadScene("Dumfries and Galloway Napoleonic Voyage");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag2)
        {
            SceneManager.LoadScene("Submerged Landscaped Mesolithic Voyage");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag3)
        {
            SceneManager.LoadScene("Unpath Co-Design Voyage");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else if (item.tag == tag4)
        {
            SceneManager.LoadScene("Women and Shipping in the 20th Century");
            Debug.Log("Interacted item's tag: " + item.tag);
        }
        else
        {
            Debug.Log("No matching tag found.");
        }
    }
}
