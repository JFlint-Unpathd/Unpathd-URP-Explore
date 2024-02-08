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

        interactable.onHoverEntered.AddListener(SliderHover);
        interactable.onSelectEntered.AddListener(StartGrabbing);
        interactable.onSelectExited.AddListener(StopGrabbing);
    }




    private void SliderHover(XRBaseInteractor interactor)
    {
        
    }
    private void StartGrabbing(XRBaseInteractor interactor)
    {
    }
    private void StopGrabbing(XRBaseInteractor interactor)
    {

    }






}
