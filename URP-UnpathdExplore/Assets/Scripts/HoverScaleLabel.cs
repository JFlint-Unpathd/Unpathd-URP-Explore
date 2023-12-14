using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class HoverScaleLabel : MonoBehaviour
{
    public XRBaseInteractable interactable;
    public LayoutElement layoutElement; 
    public GameObject titlePanel;
    public GameObject infoPanel;

    private bool resultSelected = false;

    private float originalWidth, originalHeight;
    
    private List<UnpathResource> allObjects = new List<UnpathResource>();
    private UnpathResource selectedObject;
    private SqliteController sqliteController;

    // Move the declaration of allQResults here
    private List<UnpathResource> allQResults;

    private void Awake()
    {
        infoPanel.SetActive(false);

        // Find all objects of type UnpathResource in the scene
        allObjects.AddRange(GameObject.FindObjectsOfType<UnpathResource>());

        if (layoutElement == null)
        {
            Debug.LogError("LayoutElement not assigned in the inspector.");
            return;
        }

        // Store the original preferredWidth and preferredHeight
        originalWidth = layoutElement.preferredWidth;
        originalHeight = layoutElement.preferredHeight;

        // Add listeners for the hover events
        interactable.hoverEntered.AddListener(EnlargeLayout);
        interactable.hoverExited.AddListener(ShrinkLayout);
        interactable.onSelectEntered.AddListener(ResultSelected);
        interactable.onSelectExited.AddListener(ResultDeSelected);
    }

    private void Start()
    {
        // Use Start instead of Awake to ensure that SqliteController has been instantiated
        sqliteController = FindObjectOfType<SqliteController>();
        if (sqliteController == null)
        {
            Debug.LogError("SqliteController not found in the scene.");
            return;
        }
        
        // Access the allQResults list through the SqliteController
        allQResults = sqliteController.GetAllQResults();
    }

    private void EnlargeLayout(HoverEnterEventArgs args)
    {
        // Change the preferredWidth and preferredHeight to 50 and 30 respectively
        layoutElement.preferredWidth = 50;
        layoutElement.preferredHeight = 30;
    }

    private void ShrinkLayout(HoverExitEventArgs args)
    {
        // Revert the preferredWidth and preferredHeight back to the original values
        layoutElement.preferredWidth = originalWidth;
        layoutElement.preferredHeight = originalHeight;
    }

    private void ResultSelected(XRBaseInteractor interactor)
    {
        resultSelected = !resultSelected;

        if(!resultSelected)
        {
            selectedObject = allObjects.Find(obj => obj.gameObject == interactor.gameObject);

            if (selectedObject == null)
            {
                Debug.Log("UnpathResource component not found in the interactor object");
            }
            else
            {
                InfoPanelOn();
            }
            
        }

        else
        {
            InfoPanelOff();
        }
    }

    private void ResultDeSelected(XRBaseInteractor interactor)
    {
        
    }

    private void InfoPanelOn()
    {
        Debug.Log("InfoPanelOn method called");  // Debug log for checking method call

        titlePanel.SetActive(false);
        // Set the child panel active
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }

        foreach (var obj in allQResults)
        {
            // Deactivate all objects except the selected one
            obj.gameObject.SetActive(obj == selectedObject);
        }
    }

    private void InfoPanelOff()
    {
        // Set the original panel active
        titlePanel.SetActive(true);

        // Set the child panel inactive
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }

        // Reactivate all objects
        foreach (var obj in allQResults)
        {
            obj.gameObject.SetActive(true);
        }
    }
}
