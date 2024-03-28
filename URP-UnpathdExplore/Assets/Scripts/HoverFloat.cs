using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverFloat : MonoBehaviour
{
 
    private XRBaseInteractable xrInteractable;

    private PrefabInstantiator prefabInstantiator;
    private ParentObjectController parentObjectController; 
    private SpawnAndToggle spawnAndToggle;

    public float hoverHeight = 0.5f;
    public float waitDuration = 10f;
    public float returnDelay = 5f;
    public float floatSpeed = 3f;

    private bool isCoroutineRunning = false;

    void Start()
    {

        xrInteractable = GetComponent<XRBaseInteractable>();

        prefabInstantiator = GetComponent<PrefabInstantiator>();
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
            parentObjectController.isHovered = true;

            prefabInstantiator.ResetRotation();
            parentObjectController.FreezeRotation();
            
            if (!isCoroutineRunning)
            {
                StartCoroutine(HoverRoutine());
            }
   
    }


    IEnumerator HoverRoutine()
    {
        isCoroutineRunning = true;

        //Debug.Log("HoverRoutine started");
        parentObjectController.SetKinematic(true);

        Vector3 targetPosition = prefabInstantiator.GetOriginalPosition() + Vector3.up * hoverHeight;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            //Debug.Log("Current position: " + transform.position + " Target position: " + targetPosition);
            transform.position = Vector3.Lerp(transform.position, targetPosition, floatSpeed * Time.deltaTime);
            
            yield return null;
        }

        //Debug.Log("Hover height reached, waiting for " + waitDuration + " seconds");
        yield return new WaitForSeconds(waitDuration);
        Debug.Log("Wait duration finished"); 

        if (!parentObjectController.isGrabbed && !parentObjectController.isSnapped)
        {
            Debug.Log("Starting ReturnRoutine");
            StartCoroutine(ReturnRoutine());
        }

        isCoroutineRunning = false;
    }

    void StopHover(HoverExitEventArgs args)
    {
        parentObjectController.isHovered = false;
    }

    IEnumerator ReturnRoutine()
    {
        isCoroutineRunning = true;

        spawnAndToggle.DisableSpawnedObjects();

        yield return new WaitForSeconds(returnDelay);

        Vector3 targetPosition = prefabInstantiator.GetOriginalPosition();
        
        Debug.Log("Target position at start of ReturnRoutine: " + targetPosition);

        while (Vector3.Distance(transform.position, targetPosition) > 0.0001f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, floatSpeed * Time.deltaTime);
            yield return null;
        }

        prefabInstantiator.ResetRotation();
        parentObjectController.FreezeRotation();

        isCoroutineRunning = false;

        Debug.Log("Finished Return Routine");

    }

    void StartGrab(SelectEnterEventArgs args)
    {
        parentObjectController.SetKinematic(false);
        //Debug.Log("Gravity enabled at StartGrab");
    }

    void StopGrab(SelectExitEventArgs args)
    {
        parentObjectController.isGrabbed = false;

        if (!parentObjectController.isGrabbed && !parentObjectController.isSnapped)
        {
        
            parentObjectController.SetKinematic(false);
            StartCoroutine(EndGrabRoutine());
        }
    }

    IEnumerator EndGrabRoutine()
    {
        //allow for object to be tossed before returning to orinal pos
        yield return new WaitForSeconds(returnDelay);

        parentObjectController.SetKinematic(true);
        Debug.Log("Gravity disabled at EndGrabRoutine");
        if (!parentObjectController.isGrabbed && !parentObjectController.isSnapped && !isCoroutineRunning)
        {
            StartCoroutine(ReturnRoutine());
        }
    }
}
