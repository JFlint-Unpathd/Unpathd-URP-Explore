
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ItemSelectionController : MonoBehaviour
{
    public LayoutElement layoutElement; 
    public GameObject titlePanel;
    private GameObject infoPanel; 
    
    private float originalWidth, originalHeight;

    private XRBaseInteractable interactable;
    private SqliteController sqliteController;
    private UnpathResource thisResource;

    private void Awake()
    {
        //infoPanel.SetActive(false);

        if (layoutElement == null)
        {
            Debug.LogError("LayoutElement not assigned in the inspector.");
            return;
        }

        // Store the original preferredWidth and preferredHeight
        originalWidth = layoutElement.preferredWidth;
        originalHeight = layoutElement.preferredHeight;
        
    }


    void Start()
    {

        interactable = GetComponent<XRBaseInteractable>();
        sqliteController = FindObjectOfType<SqliteController>();
        thisResource = GetComponent<UnpathResource>();

        if (sqliteController == null)
        {
            Debug.LogError("SqliteController not found in the scene.");
            return;
        }

        if (thisResource == null)
        {
            Debug.LogError("UnpathResource not found in the current object.");
            return;
        }

        // Add listeners for the hover events
        //interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.hoverEntered.AddListener(EnlargeLayout);
        interactable.hoverExited.AddListener(ShrinkLayout);
        interactable.selectEntered.AddListener(OnShowInfo);
        
        GetComponentInChildren<TMPro.TMP_Text>().text = thisResource.m_Title;
        
    }


    private void EnlargeLayout(HoverEnterEventArgs args)
    {
        // Change the preferredWidth and preferredHeight to 50 and 30 respectively
        layoutElement.preferredWidth = 50;
        layoutElement.preferredHeight = 30;

        // Scale up the mesh visually
        thisResource.transform.localScale = new Vector3(2f, 2f, 2f);
    }

    private void ShrinkLayout(HoverExitEventArgs args)
    {
        // Revert the preferredWidth and preferredHeight back to the original values
        layoutElement.preferredWidth = originalWidth;
        layoutElement.preferredHeight = originalHeight;

        // Revert the scale of the mesh visually
        thisResource.transform.localScale = Vector3.one; // Assuming original scale was (1, 1, 1)
    }


    private void OnShowInfo( SelectEnterEventArgs args ) 
    {
        List<UnpathResource> allQResults = sqliteController.GetAllQResults();
        if( InfoPanel.Show( transform, thisResource.m_Title, thisResource.m_Description, thisResource.m_Placename ) ) {
            titlePanel.SetActive( false );
            foreach( var item in allQResults ) {
                // Disable all objects except the selected one
                item.gameObject.SetActive( item == thisResource );
            }
        } else {
            titlePanel.SetActive( true );
            foreach( var obj in allQResults ) {
                obj.gameObject.SetActive( true );
            }
        }
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
    }
}
