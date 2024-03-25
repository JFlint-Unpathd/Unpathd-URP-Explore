using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnChild : MonoBehaviour
{
    public UnityEvent childGrabbed;
    public UnityEvent childReleased;

    void Start()
    {
        var xrGrabInteractable = GetComponent<XRGrabInteractable>();

        xrGrabInteractable.onSelectEntered.AddListener((interactor) => {
            childGrabbed.Invoke();
        });

        xrGrabInteractable.onSelectExited.AddListener((interactor) => {
            childReleased.Invoke();
        });
    }
}
