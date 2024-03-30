using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Handle : MonoBehaviour
{
    private XRBaseInteractable interactable;

    private void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();

        interactable.hoverEntered.AddListener(SliderHover);
        interactable.selectEntered.AddListener(StartGrabbing);
        interactable.selectExited.AddListener(StopGrabbing);
    }




    private void SliderHover(HoverEnterEventArgs args)
    {
        
    }
    private void StartGrabbing(SelectEnterEventArgs args)
    {
    }
    private void StopGrabbing(SelectExitEventArgs args)
    {

    }

}
