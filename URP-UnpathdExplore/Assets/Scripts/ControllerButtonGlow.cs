using System;
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
    public bool isOnPod;
    private bool disableTeleportationStarted = false;
    private bool grabObjectInstantiated = false;

    [Header("PromptPrefab")]
    [SerializeField] private GameObject teleportationPodPrefab;
    private GameObject teleportationPod;
    [SerializeField] private GameObject grabObjectPrefab;
    private GameObject grabObject;
    [SerializeField] private GameObject promptMenuObject;
    private GameObject promptMenu;

    [SerializeField] private GameObject menuSceneObjectPrefab;
    private GameObject menuSceneObject;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip teleportClip;
    [SerializeField] private AudioClip selectClip;
    [SerializeField] private AudioClip menuClip;
    
    // Tutorial Stage
    private enum TutorialStage 
    {
        Teleport,
        Select,
        Menu
    }

    private TutorialStage currentTutorialStage = TutorialStage.Teleport;


    private void OnTriggerEnter(Collider other)
    {
        // Check if the XR Rig has collided with the teleportation pod
        if (other.gameObject.CompareTag("TeleportationPod"))
        {
            isOnPod = true;
            Debug.Log("OnTriggerEnter: " + other.gameObject.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the XR Rig has exited the teleportation pod
        if (other.gameObject.CompareTag("TeleportationPod"))
        {
            isOnPod = false;
            Debug.Log("OnTriggerExit: " + other.gameObject.tag);
        }
    }

    private bool IsOnPod()
    {
        return isOnPod;
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

        private void Start()
    {
        // Set initial materials
        SetMaterial(triggerRenderer, glowMaterial);
        SetMaterial(primaryBtnRenderer, glowMaterial);
        SetMaterial(thumbStickRenderer, glowMaterial);

        teleportationPod = Instantiate(teleportationPodPrefab, new Vector3(0, 0, 3), Quaternion.identity);
        teleportationPod.SetActive(true);
    }


    private void ThumbStickPressed(InputAction.CallbackContext obj)
    {
        switch (currentTutorialStage)
        {
            case TutorialStage.Teleport:
                if (!thumbStickPressed && IsOnPod())
                {
                    disableTeleportationStarted = true;
                    StartCoroutine(DisableTeleportationFeatures());
                }
                break;
            case TutorialStage.Select:
                // Ignore, not this stage's action
                break;
            case TutorialStage.Menu:
                // Ignore, not this stage's action
                break;
        }
    }
    private void ThumbStickCancelled(InputAction.CallbackContext obj)
    {
        if (!thumbStickPressed)
        {
            SetMaterial(thumbStickRenderer, glowMaterial);
        }
    }

    private IEnumerator DisableTeleportationFeatures()
    {
        yield return new WaitForSeconds(1f);
        
        SetMaterial(thumbStickRenderer, normalMaterial);
        thumbStickLabel.SetActive(false);
        thumbStickPressed = true;

        // Deactivate teleport anchor after teleportation
        teleportationPod.SetActive(false);

        // Instantiate and activate grab object prefab
        if (!grabObjectInstantiated)
        {
            grabObject = Instantiate(grabObjectPrefab, new Vector3(0, 0, 6), Quaternion.identity); // modify this line
            grabObject.SetActive(true);
            grabObjectInstantiated = true;
        }

        currentTutorialStage = TutorialStage.Select;

        AudioManager.instance.PlayClip(selectClip);

    }

    private void TriggerPressed(InputAction.CallbackContext obj)
    {
        switch (currentTutorialStage)
        {
            case TutorialStage.Teleport:
                // Ignore, not this stage's action
                break;
            case TutorialStage.Select:
                if (!triggerPressed)
                {
        
                    // Disable teleportation light and panel after delay
                    SetMaterial(triggerRenderer, normalMaterial);
                    triggerLabel.SetActive(false);
                    currentTutorialStage = TutorialStage.Menu;

                    // Deactivate the grabObject
                    grabObject.SetActive(false);

                    // Instantiate and activate promptMenuObject
                    promptMenu = Instantiate(promptMenuObject, new Vector3(0, 1, 5), Quaternion.identity);
                    promptMenu.SetActive(true);

                    // Play the menu tutorial audio
                    AudioManager.instance.PlayClip(menuClip);
                    
                }
                break;
         
            case TutorialStage.Menu:
            // Ignore as it's not this stage's action
                break;
        }
    }

    
    private void TriggerCanceled(InputAction.CallbackContext obj)
    {
        if (!triggerPressed)
        {
            //SetMaterial(triggerRenderer, glowMaterial);
        }
    }

    private void PrimaryPressed(InputAction.CallbackContext obj)
    {
        switch (currentTutorialStage)
        {
            case TutorialStage.Teleport:
                // Ignore, not this stage's action
                break;
            case TutorialStage.Select:
                // Ignore, not this stage's action
                break;
            case TutorialStage.Menu:
                if (!primaryPressed)
                {
                    if (promptMenu != null)
                    {
                        promptMenu.SetActive(false);
                    }
                    SetMaterial(primaryBtnRenderer, normalMaterial);
                    primaryBtnLabel.SetActive(false);
                    primaryPressed = true;
                    StartCoroutine(DemoFinished());
                }
                break;
        }
    }

    private void PrimaryCancelled(InputAction.CallbackContext obj)
    {
        if (!primaryPressed)
        {
            SetMaterial(primaryBtnRenderer, glowMaterial);
        }
    }
    
    private IEnumerator DemoFinished()
    {
        yield return new WaitForSeconds(1f);
        GameObject inGameMenu = GameObject.FindWithTag("InGameMenu");
        if (inGameMenu != null)
        {
            inGameMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("myObject is not assigned.");
        }

        menuSceneObject = Instantiate(menuSceneObjectPrefab, new Vector3(0, 1, 7), Quaternion.identity);
        menuSceneObject.SetActive(true);

    }


}
