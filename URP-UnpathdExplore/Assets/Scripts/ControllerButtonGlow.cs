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

    [Header("Labels")]
    [SerializeField] private GameObject triggerLabel;
    [SerializeField] private GameObject primaryBtnLabel;
    [SerializeField] private GameObject thumbStickLabel;

    private bool triggerPressed = false;
    private bool primaryPressed = false;
    private bool thumbStickPressed = false;

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
        // trigger
        triggerButtonReference.action.performed += TriggerPressed;
        triggerButtonReference.action.canceled += TriggerCanceled;

        // primary
        primaryBtnReference.action.performed += PrimaryPressed; 
        primaryBtnReference.action.canceled += PrimaryCancelled;

        // thumbstick
        thumbStickReference.action.performed += ThumbStickPressed;
        thumbStickReference.action.canceled += ThumbStickCancelled;
    }

    private void OnDisable()
    {
        // Do not reset materials on disable
    }

    private void TriggerPressed(InputAction.CallbackContext obj)
    {
        if (!triggerPressed)
        {
            SetMaterial(triggerRenderer, normalMaterial);
            triggerLabel.SetActive(false);
            triggerPressed = true;
        }
    }

    private void TriggerCanceled(InputAction.CallbackContext obj)
    {
        if (!triggerPressed)
        {
            SetMaterial(triggerRenderer, glowMaterial);
        }
    }

    private void PrimaryPressed(InputAction.CallbackContext obj)
    {
        if (!primaryPressed)
        {
            SetMaterial(primaryBtnRenderer, normalMaterial);
            primaryBtnLabel.SetActive(false);
            primaryPressed = true;
        }
    }

    private void PrimaryCancelled(InputAction.CallbackContext obj)
    {
        if (!primaryPressed)
        {
            SetMaterial(primaryBtnRenderer, glowMaterial);
        }
    }

    private void ThumbStickPressed(InputAction.CallbackContext obj)
    {
        if (!thumbStickPressed)
        {
            SetMaterial(thumbStickRenderer, normalMaterial);
            thumbStickLabel.SetActive(false);
            thumbStickPressed = true;
        }
    }

    private void ThumbStickCancelled(InputAction.CallbackContext obj)
    {
        if (!thumbStickPressed)
        {
            SetMaterial(thumbStickRenderer, glowMaterial);
        }
    }
}
