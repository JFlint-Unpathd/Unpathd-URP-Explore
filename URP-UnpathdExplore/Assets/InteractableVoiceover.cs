using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractableVoiceover : MonoBehaviour
{
    public AudioClip voiceoverClip;
    private AudioSource audioSource;
    private XRGrabInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();
        interactable.onSelectEntered.AddListener(OnSelectEntered);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = voiceoverClip;
    }

    private void OnSelectEntered(XRBaseInteractor interactor)
    {
        audioSource.Play();
    }
}
