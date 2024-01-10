// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;

// public class BirdsEyeView : MonoBehaviour
// {
//     private XRGrabInteractable grabInteractable;

//     private void Start()
//     {
//         // Ensure there is an XRGrabInteractable component on the object
//         grabInteractable = GetComponent<XRGrabInteractable>();
//         if (grabInteractable == null)
//         {
//             Debug.LogError("XRGrabInteractable component not found on the birdsEye object.");
//             return;
//         }

//         // Subscribe to the selection event
//         grabInteractable.onSelectEntered.AddListener(OnSelected);
//     }

//     private void OnSelected(XRBaseInteractor interactor)
//     {
//         // Find the XR Rig in the scene
//         XRRig xrRig = FindObjectOfType<XRRig>();
//         if (xrRig == null)
//         {
//             Debug.LogWarning("XR Rig not found in the scene. Make sure it is present and active.");
//             return;
//         }

//         Debug.Log("XR Rig found");
//         // Update the XR Rig's position to match the bird's position
//         xrRig.transform.position = transform.position;

//         // You might also want to set the rotation if needed
//         // xrRig.transform.rotation = transform.rotation;
//     }
// }
