using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverFloat : MonoBehaviour
{
 
    private XRBaseInteractable xrInteractable;

    private TransformKeeper transformKeeper;
    private ParentObjectController parentObjectController; 
    private SpawnAndToggle spawnAndToggle;

    private float hoverHeight = 1f;
    public float waitDuration = 10f;
    public float returnDelay = 5f;
    public float floatSpeed = 3f;

    private bool isCoroutineRunning = false;

    void Start()
    {

        xrInteractable = GetComponent<XRBaseInteractable>();

        transformKeeper = GetComponent<TransformKeeper>();
        parentObjectController = GetComponent<ParentObjectController>();
        spawnAndToggle = GetComponent<SpawnAndToggle>();

        // Listners 
        xrInteractable.hoverEntered.AddListener(StartHover);
        xrInteractable.hoverExited.AddListener(StopHover);
        xrInteractable.selectEntered.AddListener(StartGrab);
        xrInteractable.selectExited.AddListener(StopGrab);
        
    }

    void StartHover(HoverEnterEventArgs args)
    {  
        if (parentObjectController != null) {

            parentObjectController.isHovered = true;

            transformKeeper.ResetRotation();
            
            parentObjectController.FreezeRotation();
    
        }
            
        if (!isCoroutineRunning)
        {
            StartCoroutine(HoverRoutine());
        }
   
    }


    IEnumerator HoverRoutine()
    {
        isCoroutineRunning = true;

        if (parentObjectController != null) {

          //Debug.Log("HoverRoutine started");
            parentObjectController.SetKinematic(true);
    
        }


        Vector3 targetPosition = transformKeeper.GetOriginalPosition() + Vector3.up * hoverHeight;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            //Debug.Log("Current position: " + transform.position + " Target position: " + targetPosition);
            transform.position = Vector3.Lerp(transform.position, targetPosition, floatSpeed * Time.deltaTime);
            
            yield return null;
        }

        //Debug.Log("Hover height reached, waiting for " + waitDuration + " seconds");
        yield return new WaitForSeconds(waitDuration);
        //Debug.Log("Wait duration finished"); 

        if (parentObjectController != null) {

            if (!parentObjectController.isGrabbed && !parentObjectController.isSnapped)
            {
                //Debug.Log("Starting ReturnRoutine");
                StartCoroutine(ReturnRoutine());
            }
    
        }

       

        isCoroutineRunning = false;
    }

    void StopHover(HoverExitEventArgs args)
    {
         if (parentObjectController != null) {

          parentObjectController.isHovered = false;
    
        }
          
    }

    IEnumerator ReturnRoutine()
    {
        isCoroutineRunning = true;

        yield return new WaitForSeconds(returnDelay);

        Vector3 targetPosition = transformKeeper.GetOriginalPosition();
        
        //Debug.Log("Target position at start of ReturnRoutine: " + targetPosition);

        while (Vector3.Distance(transform.position, targetPosition) > 0.0001f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, floatSpeed * Time.deltaTime);
            yield return null;
        }

        transformKeeper.ResetRotation();

        if (parentObjectController != null) {

          parentObjectController.FreezeRotation();
    
        }

               if (spawnAndToggle != null) {

            //spawnAndToggle.DisableSpawnedObjects();
            spawnAndToggle.ToggleSpawnedObjectsVisibility();
    
        }
          
        isCoroutineRunning = false;

        //Debug.Log("Finished Return Routine");

    }

    void StartGrab(SelectEnterEventArgs args)
    {
        
        
        if (parentObjectController != null) {

        parentObjectController.SetKinematic(false);
        //Debug.Log("Gravity enabled at StartGrab");
        }
    
    }

    void StopGrab(SelectExitEventArgs args)
    {
        if (parentObjectController != null) {

        parentObjectController.isGrabbed = false;

        if (!parentObjectController.isGrabbed && !parentObjectController.isSnapped)
        {
        
            parentObjectController.SetKinematic(false);
            //StartCoroutine(EndGrabRoutine());

            if (!parentObjectController.isGrabbed && !parentObjectController.isSnapped && !isCoroutineRunning)
            {
                StartCoroutine(ReturnRoutine());
            }
        }

        }
     
    }

    IEnumerator EndGrabRoutine()
    {
        //allow for object to be tossed before returning to orinal pos
        yield return new WaitForSeconds(returnDelay);

       if (parentObjectController != null) {

            //Debug.Log("Gravity disabled at EndGrabRoutine");

            if (!parentObjectController.isGrabbed && !parentObjectController.isSnapped && !isCoroutineRunning)
            {
                StartCoroutine(ReturnRoutine());
            }
        }
    }
}
