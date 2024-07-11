using System;
using System.Collections;
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
    [SerializeField] private GameObject secondaryTriggerLabel;
    [SerializeField] private GameObject primaryBtnLabel;
    [SerializeField] private GameObject thumbStickLabel;

    [Header("PromptPrefab")]
    [SerializeField] private GameObject teleportationPodPrefab;
    private GameObject teleportationPod;
    [SerializeField] private GameObject selectObjectPrefab;
    private GameObject selectObject;
    [SerializeField] private GameObject grabObjectPrefab;
    private GameObject grabObject;
    [SerializeField] private GameObject menuSceneObjectPrefab;
    private GameObject menuSceneObject;
    [SerializeField] private GameObject promptMenuObject;
    private GameObject promptMenu;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip teleportClip;
    [SerializeField] private AudioClip selectClip;
    [SerializeField] private AudioClip grabClip;
    [SerializeField] private AudioClip menuClip;

    private enum TutorialStage
    {
        Teleport,
        Select,
        Grab,
        Menu
    }

    private TutorialStage currentTutorialStage = TutorialStage.Teleport;

    private bool triggerPressed = false;
    private bool primaryPressed = false;
    private bool grabReleased = false;
    private bool thumbStickPressed = false;
    public bool isOnPod;
    private bool disableTeleportationStarted = false;
    private bool selectObjectInstantiated = false;
    private bool canTransitionToGrab = false;
    private bool hasGrabbedObject = false;
    private bool isSelectStageFinished = false;

    private Coroutine grabReminderCoroutine;
    private GameObject reminderPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TeleportationPod"))
        {
            isOnPod = true;
        }
        else if (other.gameObject == grabObject)
        {
            hasGrabbedObject = true; // Set true when the grab object is interacted with
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("TeleportationPod"))
        {
            isOnPod = false;
        }
    }

    public bool IsOnPod()
    {
        return isOnPod;
    }

    private void SetMaterial(MeshRenderer renderer, Material material)
    {
        renderer.material = material;
    }

    private void Awake()
    {
        triggerButtonReference.action.performed += TriggerPressed;
        triggerButtonReference.action.canceled += TriggerCanceled;
        primaryBtnReference.action.performed += PrimaryPressed;
        primaryBtnReference.action.canceled += PrimaryCancelled;
        thumbStickReference.action.performed += ThumbStickPressed;
        thumbStickReference.action.canceled += ThumbStickCancelled;
    }

    private void Start()
    {
        SetMaterial(triggerRenderer, glowMaterial);
        SetMaterial(primaryBtnRenderer, glowMaterial);
        SetMaterial(thumbStickRenderer, glowMaterial);

        teleportationPod = Instantiate(teleportationPodPrefab, new Vector3(0, 0, 3), Quaternion.identity);
        teleportationPod.SetActive(true);

        secondaryTriggerLabel.SetActive(false);
    }

    private void TransitionToGrabStage()
    {
        StartCoroutine(HandleGrabStageTransition());
    }

    private IEnumerator HandleGrabStageTransition()
    {
        // Add a delay before transitioning to the grab stage
        yield return new WaitForSeconds(2f);

        grabObject = Instantiate(grabObjectPrefab, new Vector3(0, 1, 6), Quaternion.identity);
        grabObject.SetActive(true);

        // Find and deactivate the reminder panel
        reminderPanel = grabObject.transform.Find("Reminder Panel")?.gameObject;
        if (reminderPanel != null)
        {
            reminderPanel.SetActive(false);
        }

        SetMaterial(triggerRenderer, glowMaterial);
        secondaryTriggerLabel.SetActive(true);

        currentTutorialStage = TutorialStage.Grab;
        AudioManager.instance.PlayClip(grabClip);
    }

    private void ThumbStickPressed(InputAction.CallbackContext obj)
    {
        if (currentTutorialStage == TutorialStage.Teleport && !thumbStickPressed && IsOnPod())
        {
            disableTeleportationStarted = true;
            StartCoroutine(DisableTeleportationFeatures());
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
        teleportationPod.SetActive(false);

        if (!selectObjectInstantiated)
        {
            selectObject = Instantiate(selectObjectPrefab, new Vector3(0, 0, 6), Quaternion.identity);
            selectObject.SetActive(true);
            selectObjectInstantiated = true;
        }

        currentTutorialStage = TutorialStage.Select;
        AudioManager.instance.PlayClip(selectClip);

        // Example: Set the select stage as finished
        isSelectStageFinished = true;
    }

    private void TriggerPressed(InputAction.CallbackContext obj)
    {
        // Check if in Select stage, selection is finished, and grabObject is ready
        if (currentTutorialStage == TutorialStage.Select && !triggerPressed)
        {
            SetMaterial(triggerRenderer, normalMaterial);
            triggerLabel.SetActive(false);

            if (selectObject != null)
            {
                selectObject.SetActive(false);
            }

            SetMaterial(triggerRenderer, glowMaterial);
            secondaryTriggerLabel.SetActive(true);

            canTransitionToGrab = true;

            if (isSelectStageFinished)
            {
                TransitionToGrabStage();
            }
        }
        else if (currentTutorialStage == TutorialStage.Grab && !hasGrabbedObject)
        {
            if (grabReminderCoroutine != null)
            {
                StopCoroutine(grabReminderCoroutine);
            }
            grabReminderCoroutine = StartCoroutine(ShowReleaseReminder());
            Debug.Log("Handling Grab Stage Trigger Press");
        }
    }

    private void TriggerCanceled(InputAction.CallbackContext obj)
    {
        if (this == null || !this.gameObject.activeInHierarchy) 
        {
            Debug.Log("ControllerButtonGlow object is no longer active.");
            return;
        }
        
        Debug.Log("Trigger Released");
        if (grabReminderCoroutine != null)
        {
            StopCoroutine(grabReminderCoroutine);
        }

        if (currentTutorialStage == TutorialStage.Grab && grabObject != null && grabObject.activeSelf)
        {
            grabReleased = true;
            if (reminderPanel != null)
            {
                reminderPanel.SetActive(false);
            }

            SetMaterial(triggerRenderer, normalMaterial);
            secondaryTriggerLabel.SetActive(false);

            if (grabObject != null)
            {
                grabObject.SetActive(false);
            }

            promptMenu = Instantiate(promptMenuObject, new Vector3(0, 1, 5), Quaternion.identity);
            promptMenu.SetActive(true);

            AudioManager.instance.PlayClip(menuClip);
            currentTutorialStage = TutorialStage.Menu;
        }
    }

    private IEnumerator ShowReleaseReminder()
    {
        yield return new WaitForSeconds(3f);
        if (!grabReleased && currentTutorialStage == TutorialStage.Grab)
        {
            if (reminderPanel != null)
            {
                reminderPanel.SetActive(true);
            }
        }
    }

    private void PrimaryPressed(InputAction.CallbackContext obj)
    {
        Debug.Log($"Primary Button Pressed: current stage = {currentTutorialStage}, primaryPressed = {primaryPressed}");

        if (currentTutorialStage == TutorialStage.Menu && !primaryPressed)
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
        yield return new WaitForSeconds(3f);
        GameObject inGameMenu = GameObject.FindWithTag("InGameMenu");
        if (inGameMenu != null)
        {
            inGameMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("InGameMenu is not assigned.");
        }

        menuSceneObject = Instantiate(menuSceneObjectPrefab, new Vector3(0, 1, 7), Quaternion.identity);
        menuSceneObject.SetActive(true);
    }
}
