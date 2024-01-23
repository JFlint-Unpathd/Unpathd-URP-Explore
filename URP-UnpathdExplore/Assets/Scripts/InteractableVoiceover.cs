using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableVoiceover : MonoBehaviour
{
    public AudioClip voiceoverClip;
    private XRGrabInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();
        interactable.onSelectEntered.AddListener(OnSelectEntered);

    }

    private void OnSelectEntered(XRBaseInteractor interactor)
    {
        AudioManager.instance.PlayClip(voiceoverClip);
    }

}
