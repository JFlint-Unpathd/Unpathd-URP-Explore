using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnedObjectScript : MonoBehaviour
{
    public UnityEvent childGrabbed, childReleased;
    
    void Start()
    {
        var xrGrabInteractable = GetComponent<XRGrabInteractable>();

        xrGrabInteractable.onSelectEntered.AddListener((interactor) => childGrabbed.Invoke());
        xrGrabInteractable.onSelectExited.AddListener((interactor) => childReleased.Invoke());
    }
}
