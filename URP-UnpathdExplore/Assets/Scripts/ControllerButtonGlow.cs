using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerButtonGlow : MonoBehaviour
{
    [Header("Input Actions for Buttons")]
    [SerializeField] private InputActionReference triggerButtonReference;
    [SerializeField] private InputActionReference primaryBtnReference;
    [SerializeField] private InputActionReference thumbStickReference;

    [Header("Mesh Renderer for Buttons")]
    [SerializeField] private MeshRenderer triggerRenderer;
    [SerializeField] private MeshRenderer primaryBtnRenderer;
    [SerializeField] private MeshRenderer thumbStickRenderer;

     [Header("Materials")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material glowMaterial;

     private void Start()
    {
        // Set initial materials
        SetMaterial(triggerRenderer, glowMaterial);
        SetMaterial(primaryBtnRenderer, glowMaterial);
        SetMaterial(thumbStickRenderer, glowMaterial);
    }

    private void SetMaterial(MeshRenderer renderer, Material material)
    {
        renderer.material = material;
    }

    void Awake()
    {
        //trigger
        triggerButtonReference.action.performed += TriggerPressed;
        triggerButtonReference.action.canceled += TriggerCanceled;

        //primary
        primaryBtnReference.action.performed += PrimaryPressed; 
        thumbStickReference.action.canceled += PrimaryCancelled;

        //thumbstick
        thumbStickReference.action.performed += ThumbStickPressed;
        thumbStickReference.action.performed += ThumbStickCancelled;
    }

    // private void OnDisable()
    // {
   
    //     //trigger
    //     triggerButtonReference.action.performed -= TriggerPressed;
    //     triggerButtonReference.action.canceled -= TriggerCanceled;

    //     //primary
    //     primaryBtnReference.action.performed -= PrimaryPressed; 
    //     thumbStickReference.action.canceled -= PrimaryCancelled;

    //     //thumbstick
    //     thumbStickReference.action.performed -= ThumbStickPressed;
    //     thumbStickReference.action.performed -= ThumbStickCancelled;
    // }
    
     private void OnDisable()
    {
        // Reset materials on disable
        SetMaterial(triggerRenderer, normalMaterial);
        SetMaterial(primaryBtnRenderer, normalMaterial);
        SetMaterial(thumbStickRenderer, normalMaterial);
    }

    // #region Trigger
    // private void TriggerPressed(InputAction.CallbackContext obj) => triggerRenderer.enabled = true;
    // private void TriggerCanceled(InputAction.CallbackContext obj) => triggerRenderer.enabled = false;
    // #endregion

    // #region PrimaryBtn
    // private void PrimaryPressed(InputAction.CallbackContext obj) => primaryBtnRenderer.enabled = true;
    // private void PrimaryCancelled(InputAction.CallbackContext obj) => primaryBtnRenderer.enabled = false;
    // #endregion

    // #region Thumbstick
    // private void ThumbStickPressed(InputAction.CallbackContext obj) => thumbStickRenderer.enabled = true;
    // private void ThumbStickCancelled(InputAction.CallbackContext obj) => thumbStickRenderer.enabled = false;
    // #endregion

    private void TriggerPressed(InputAction.CallbackContext obj) => SetMaterial(triggerRenderer, normalMaterial);
    private void TriggerCanceled(InputAction.CallbackContext obj) => SetMaterial(triggerRenderer, glowMaterial);

    private void PrimaryPressed(InputAction.CallbackContext obj) => SetMaterial(primaryBtnRenderer, normalMaterial);
    private void PrimaryCancelled(InputAction.CallbackContext obj) => SetMaterial(primaryBtnRenderer, glowMaterial);

    private void ThumbStickPressed(InputAction.CallbackContext obj) => SetMaterial(thumbStickRenderer, normalMaterial);
    private void ThumbStickCancelled(InputAction.CallbackContext obj) => SetMaterial(thumbStickRenderer, glowMaterial);


}