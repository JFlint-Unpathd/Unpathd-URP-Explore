
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class ZoomController : MonoBehaviour
{
    public GameObject sqliteControllerObject;
    private SqliteController sqliteController;
    
    
    // The list of currently selected UnpathResource objects for zoom logic
    private List<UnpathResource> zoomList = new List<UnpathResource>();
    public Material selectionColor;
    public Material originalMaterial;
    public GameObject zoomObject;
    private bool isActivated = false;

    

    void Start()
    {
        sqliteController = sqliteControllerObject.GetComponent<SqliteController>();
        

        // Get the XRGrabInteractable component and set up the OnSelect event.
        XRGrabInteractable grabInteractable = zoomObject.GetComponent<XRGrabInteractable>();

        grabInteractable.hoverEntered.AddListener(HandleHoverEnter);
        //grabInteractable.hoverExited.AddListener(HandleHoverExit);
        grabInteractable.selectEntered.AddListener(HandleSelectEnter);
        //grabInteractable.selectExited.AddListener(HandleSelectExit);

        
    }


        public void ZoomList(string selectedId) {

           //Debug.Log("ZoomList method called with ID: " + selectedId);
           UnpathResource selectedResource = sqliteController.GetResourceDict()[selectedId];
            if (selectedResource.isHovered) 
            {
                zoomObject.SetActive(true);
            } 

            else {
                // Otherwise, select the resource.
                selectedResource.isHovered = true;
                zoomList.Add(selectedResource);
                //Debug.Log("Zoom List Count: " + zoomList.Count);

                // Change the material to the selectionColor.
                selectedResource.GetComponent<Renderer>().material = selectionColor;
            }
        }

        
        
        private void HandleHoverEnter(HoverEnterEventArgs args)
        {
            Debug.Log("Grabbed cube");            
            // Toggle the activation status each time the interactable is selected
            isActivated = !isActivated;
            ToggleActivation(isActivated);
        }

    
        private void HandleSelectEnter(SelectEnterEventArgs args) {

            // Toggle the activation status each time the interactable is selected, to show refined items/ show all items
            isActivated = !isActivated;
            ToggleActivation(isActivated);
            if (isActivated) {
                ClearSelection();
            }
        }

  

        public void ToggleActivation(bool activate) 
        {
            foreach (var resource in sqliteController.GetResourceDict().Values)
            {
                if (!zoomList.Contains(resource)) 
                {
                    resource.gameObject.SetActive(activate);
                }
            }
        }
        

        public void ClearSelection() {

            foreach (var selectedResource in zoomList) {
                selectedResource.isHovered = false;
                selectedResource.GetComponent<Renderer>().material = originalMaterial;
            }
            zoomList.Clear();
            
        }

}
