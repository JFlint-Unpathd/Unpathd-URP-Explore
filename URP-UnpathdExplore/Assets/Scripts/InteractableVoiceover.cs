using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//[RequireComponent(typeof(XRBaseInteractable))]
public class InteractableVoiceover : MonoBehaviour
{
    public AudioClip voiceoverClip;
    private XRBaseInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);

    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        AudioManager.instance.PlayClip(voiceoverClip);
    }

}
