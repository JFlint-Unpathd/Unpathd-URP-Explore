using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class TriggerSelect : MonoBehaviour
{
    public InputActionReference toggleReference = null;
    public HoverFloat hoverFloatScript;

    [Range(0, 1)] // Restrict the range of the haptic intensity slider
    public float hapticIntensity = 0.5f;

    private bool isFloating = false;
    private UnityEngine.XR.InputDevice rightHandDevice;
    private UnityEngine.XR.InputDevice leftHandDevice;
    private bool hasRightHandHaptics;
    private bool hasLeftHandHaptics;

    private void Awake()
    {
        toggleReference.action.started += ToggleFloat;
    }

    private void OnDestroy()
    {
        toggleReference.action.started -= ToggleFloat;
    }

    private void Start()
    {
        rightHandDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        leftHandDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        hasRightHandHaptics = rightHandDevice != null && rightHandDevice.TryGetHapticCapabilities(out HapticCapabilities rightHandCapabilities) && rightHandCapabilities.supportsImpulse;
        hasLeftHandHaptics = leftHandDevice != null && leftHandDevice.TryGetHapticCapabilities(out HapticCapabilities leftHandCapabilities) && leftHandCapabilities.supportsImpulse;
    }

    private void ToggleFloat(InputAction.CallbackContext context)
    {
        

        if (isFloating)
        {
            hoverFloatScript.StopFloating();
            isFloating = false;
        }
        else
        {
            hoverFloatScript.StartFloating();
            isFloating = true;
        }

        if (hasRightHandHaptics)
        {
            rightHandDevice.SendHapticImpulse(0, hapticIntensity, 1f); // Adjust the duration if needed
        }

        if (hasLeftHandHaptics)
        {
            leftHandDevice.SendHapticImpulse(0, hapticIntensity, 1f); // Adjust the duration if needed
        }
    }
}
