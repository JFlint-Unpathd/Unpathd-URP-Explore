// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Slider : MonoBehaviour
// {
//     [SerializeField] Handle handle;
//     [SerializeField] Mechanic control;

//     [SerializeField] Vector2 handleStartAndEndLocalZ;
//     [SerializeField] Vector2 outputRange;
//     [SerializeField] float startValue;

//     public event Action<float> OnNewSliderValue;

//     bool isGrabbed;
//     float offsetOnGrab;
//     Vector3 handleLocalStartPos;


    
//     void OnEnable()
//     {
//         handle.OnStartGrabbing += StartGrabbing;
//         handle.OnStopGrabbing += StopGrabbing;
//     }

    
//     void SetStartPosition()
//     {
//         float transformRange = Mathf.Abs(handleStartAndEndLocalZ.x - handleStartAndEndLocalZ.y);
//         var startPositionZ = handleStartAndEndLocalZ.x + (transformRange * startValue);
//         handleLocalStartPos = handle.transform.localPosition;
//         handle.transform.localPosition = new Vector3(handleLocalStartPos.x, handleLocalStartPos.y, startPositionZ);
//         OnNewSliderValue?.Invoke(startValue);
//     }

//     public void StartGrabbing(Transform grabber)
//     {
//         activeGrabber = grabber;
//         isGrabbed = true;

//         handleLocalStartPos = handle.transform.localPosition;
//         handleInLocalSpace = handle.transform.parent.InverseTransformPoint(grabber.position);
//         offsetOnGrab = handleInLocalSpace.z;
//     }

//     private void Update()
//     {
//         if(isGrabbed)
//         {
//             MoveToHand();
        
//         }
//     }

//     public void MoveToHand()
//     {
//         if(SliderRestriction == SliderRestriction.locked)
//         return;

//         float newHandPosition = handle.transform.parent.InverseTransformPoint(activeGrabber.position).z;
//         handleMovedSinceGrab = newHandPosition - offsetOnGrab;


//     }
// }
