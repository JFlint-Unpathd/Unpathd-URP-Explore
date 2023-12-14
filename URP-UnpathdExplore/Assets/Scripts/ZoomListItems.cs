// using System.Collections.Generic;
// using Mono.Data.Sqlite;
// using System.Data;
// using System.IO;
// using UnityEngine;
// using System.Collections;
// using System.Text;
// using TMPro;
// using UnityEngine.XR.Interaction.Toolkit;


// public class SqliteController : MonoBehaviour {
 
//     //added by M 
//     // The list of currently selected UnpathResource objects for zoom logic
//     private List<UnpathResource> zoomList = new List<UnpathResource>();
//     public Material selectionColor;
//     public Material originalMaterial;
    
//     public GameObject zoomObject;
//     private bool isActivated = false;
    
//     private void Start() {
        
//         //added by M for zoom logic
//         XRGrabInteractable grabInteractable = zoomObject.GetComponent<XRGrabInteractable>();

//         grabInteractable.hoverEntered.AddListener(HandleHoverEnter);
//         grabInteractable.hoverExited.AddListener(HandleHoverExit);
//         grabInteractable.selectEntered.AddListener(HandleSelectEnter);
//         grabInteractable.selectExited.AddListener(HandleSelectExit);
//     }

    

//     public void RunQuery() 
//     {

        
//         //added by M for Zoom logic
//         //Get the XRSimpleInteractable component and set up the OnSelect event.
//         XRSimpleInteractable interactable = obj.GetComponent<XRSimpleInteractable>();
//         interactable.hoverEntered.AddListener((interactor) => {
//             ZoomList(id);
//         });

     
//     }
    

    
//     // added by M for zoom logic
//         private void ZoomList(string selectedId) {
//             UnpathResource selectedResource = m_resourceDict[selectedId];
//             if (selectedResource.isHovered) 
//             {
//                 zoomObject.SetActive(true);
//             } 

//             else {
//                 // Otherwise, select the resource.
//                 selectedResource.isHovered = true;
//                 zoomList.Add(selectedResource);

//                 // Change the material to the selectionColor.
//                 selectedResource.GetComponent<Renderer>().material = selectionColor;
//             }
//         }

        
        
//         private void HandleHoverEnter(HoverEnterEventArgs args)
//         {
//             // Toggle the activation status each time the interactable is selected
//             isActivated = !isActivated;
//             ToggleActivation(isActivated);
//         }

    
//         private void HandleSelectEnter(SelectEnterEventArgs args) {

//             // Toggle the activation status each time the interactable is selected, to show refined items/ show all items
//             isActivated = !isActivated;
//             ToggleActivation(isActivated);
//             if (isActivated) {
//                 ClearSelection();
//             }
//         }

  

//         public void ToggleActivation(bool activate) 
//         {
//             foreach (var resource in m_resourceDict.Values) 
//             {
//                 if (!zoomList.Contains(resource)) 
//                 {
//                     resource.gameObject.SetActive(activate);
//                 }
//             }
//         }
        

//         public void ClearSelection() {

//             foreach (var selectedResource in zoomList) {
//                 selectedResource.isHovered = false;
//                 selectedResource.GetComponent<Renderer>().material = originalMaterial;
//             }
//             zoomList.Clear();
            
//         }
// }

