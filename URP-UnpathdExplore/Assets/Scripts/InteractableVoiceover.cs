using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableVoiceover : MonoBehaviour
{
    public AudioClip voiceoverClip;
    private XRGrabInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);

    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        AudioManager.instance.PlayClip(voiceoverClip);
    }

}
