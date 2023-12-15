using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class UnpathSelector : MonoBehaviour, IPointerClickHandler {

    public string m_Title;
    public string m_QueryTerm;
    private SqliteController m_databaseController;

    //added by M
    private TextMeshProUGUI label_TmpPro;
    private Image imageComponent; 
    private XRGrabInteractable grabInteractable;
    private bool labelOn = false;
     


    private void Awake()
    {
        
        // label_TmpPro = GetComponentInChildren<TextMeshProUGUI>(true);
        // imageComponent = transform.Find("Interactable Label").GetComponent<Image>();
        // grabInteractable = GetComponent<XRGrabInteractable>(); 
    
    }
    
    //...

    private void Start() {
        
        m_databaseController = GameObject.FindWithTag( "DB" ).GetComponent<SqliteController>();

        label_TmpPro = GetComponentInChildren<TextMeshProUGUI>(true);
        imageComponent = GetComponentInChildren<Image>(true);
        grabInteractable = GetComponent<XRGrabInteractable>(); 
        
        //added by M
        grabInteractable.hoverEntered.AddListener(HandleHoverEnter);
        grabInteractable.hoverExited.AddListener(HandleHoverExit);
        UpdateLabelText();

        // Initially hide the label and image
        if (imageComponent != null)
        {
            imageComponent.gameObject.SetActive(false);
        }
        if (label_TmpPro != null)
        {
            label_TmpPro.gameObject.SetActive(false);
        }
 

    }

    public void OnPointerClick( PointerEventData eventData ) {
        if( eventData.button == 0 ) {
            m_databaseController.AddToQuery( m_QueryTerm, SqliteController.QueryType.Or );
            Debug.Log( $"Selected: {m_QueryTerm}" );

        } else {
            m_databaseController.RunQuery();
        }
    }

    // added by M

    public void HandleSelection()
    {   
        if (m_databaseController != null) {
            m_databaseController.AddToQuery(m_QueryTerm, SqliteController.QueryType.Or);

            // Reference the GameObject this script is attached to
            GameObject thisGameObject = gameObject;

            Debug.Log($"Selected: {m_QueryTerm} on GameObject: {thisGameObject.name}");
        }
        //added today to get all results
        else
        {
            Debug.Log("No condition to be passed.");
        }
    }

     private void UpdateLabelText() {

        label_TmpPro = GetComponentInChildren<TextMeshProUGUI>(true);

        if (label_TmpPro != null) {
            label_TmpPro.text = m_Title;
        } else {
            Debug.LogWarning("TextMeshPro component not found on the child GameObject or its descendants.");
        }
    }

    private void HandleHoverEnter(HoverEnterEventArgs args)
    {
        
        labelOn = true;

        if (labelOn && imageComponent != null)
        {
            imageComponent.gameObject.SetActive(true); // Show the Image
        }

        if (label_TmpPro != null)
        {
            label_TmpPro.gameObject.SetActive(true); // Show the label
        }
        
    }

    private void HandleHoverExit(HoverExitEventArgs args)
    {
        labelOn = false;

        if (!labelOn && imageComponent != null)
        {
            imageComponent.gameObject.SetActive(false); // Hide the Image
        }

        if (label_TmpPro != null)
        {
            label_TmpPro.gameObject.SetActive(false); // Hide the label
        }
    }

    //....


}
