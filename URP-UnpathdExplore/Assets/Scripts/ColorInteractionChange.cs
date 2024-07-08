using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ColorInteractionChange : MonoBehaviour
{
    public ColorProperties colorProperties;
    public Renderer ren;
    public XRBaseInteractable interactable;

    private bool isAudioPlaying = false;
    private bool isSoundScapeInstance = false;

    void Start()
    {
        ren.material.color = colorProperties.normalColor;

        interactable.hoverEntered.AddListener(HoverEntered);
        interactable.hoverExited.AddListener(HoverExited);
        interactable.selectEntered.AddListener(SelectEntered);
        interactable.selectExited.AddListener(SelectExited);
    }

    public void SetSoundScapeInstance(bool state)
    {
        isSoundScapeInstance = state;
    }

    public void HoverEntered(HoverEnterEventArgs args)
    {
        if (interactable.isSelected || isAudioPlaying)
            return;

        ren.material.color = colorProperties.hoverColor;
    }

    public void HoverExited(HoverExitEventArgs args)
    {
        if (interactable.isSelected || isAudioPlaying)
        {
            ren.material.color = colorProperties.snappedColor;
        }
        else
        {
            ren.material.color = colorProperties.normalColor;
        }
    }

    public void SelectEntered(SelectEnterEventArgs args)
    {
        if (isAudioPlaying)
            return;

        ren.material.color = colorProperties.selectedColor;
    }

    public void SelectExited(SelectExitEventArgs args)
    {
        if (isAudioPlaying)
            return;

        ren.material.color = colorProperties.normalColor;
    }

    public void SetPlayingColor(Color color)
    {
        if (isSoundScapeInstance)
        {
            isAudioPlaying = true;
            ren.material.color = color;
        }
    }

    public void ResetColor()
    {
        if (isSoundScapeInstance)
        {
            isAudioPlaying = false;
            ren.material.color = colorProperties.normalColor;
        }
    }
}
