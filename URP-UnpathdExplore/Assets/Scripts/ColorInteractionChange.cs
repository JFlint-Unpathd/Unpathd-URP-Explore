using UnityEngine;
using System.Collections;   
using System.Collections.Generic;   
using UnityEngine.XR.Interaction.Toolkit;

public class ColorInteractionChange : MonoBehaviour
{
   
    public ColorProperties colorProperties;
    public Renderer ren;
    public XRBaseInteractable interactable;


    void Start()
    {
        //ren.material = colorProperties.normalMat;
        ren.material.color = colorProperties.normalColor;

        interactable.hoverEntered.AddListener(HoverEntered); 
        interactable.hoverExited.AddListener(HoverExited); 
        interactable.selectEntered.AddListener(SelectEntered); 
        interactable.selectExited.AddListener(SelectExited);
    }

    void HoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("Hover Entered");

        if (interactable.isSelected)
            return;

        ren.material.color = colorProperties.hoverColor;
    }

    void HoverExited(HoverExitEventArgs args)
    {
        Debug.Log("Hover Exited");
        if (interactable.isSelected)
        {
            ren.material.color = colorProperties.snappedColor;
        }

        else
        {
            //ren.material = colorProperties.normalMat;
            ren.material.color = colorProperties.normalColor;
        }

    }

    void SelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Select Entered");
        //ren.material = colorProperties.selectedMat;
        ren.material.color = colorProperties.selectedColor;
    }

    void SelectExited(SelectExitEventArgs args)
    {
        Debug.Log("Select Exited");
        //ren.material = colorProperties.normalMat;
        ren.material.color = colorProperties.normalColor;
    } 


}
