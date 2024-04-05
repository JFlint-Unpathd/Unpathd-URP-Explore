using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class PrefabInstantiator : MonoBehaviour
{
    public UnityEvent OnTransformSaved;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    public Vector3 OriginalPosition { get { return originalPosition; } set { originalPosition = value; } }
    public Quaternion OriginalRotation { get { return originalRotation; } set { originalRotation = value; } }
    public Vector3 OriginalScale { get { return originalScale; } set { originalScale = value; } }


    public void SaveOriginalTransform()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        //Debug.Log($"Saving transform for {gameObject.name}: position {originalPosition}, rotation {originalRotation}, scale {originalScale}");
         Debug.Log("Transform saved for " + gameObject.name + ": Position - " + originalPosition + ", Rotation - " + originalRotation);
    }

    // Add methods to retrieve the original position, rotation, and scale.
    public Vector3 GetOriginalPosition()
    {
        //Debug.Log(gameObject.name + " GetOriginalPosition in ObjectTransformManager: " + originalPosition);
        return originalPosition;
    }

    public Quaternion GetOriginalRotation()
    {
        //Debug.Log(gameObject.name + " GetOriginalRotation in ObjectTransformManager: " + originalRotation);
        return originalRotation;
    }

    public Vector3 GetOriginalScale()
    {
        //Debug.Log(gameObject.name + " GetOriginalScale in ObjectTransformManager: " + originalScale);
        return originalScale;
    }

    public void ResetRotation()
    {
        transform.rotation = originalRotation;
        //Debug.Log("Resetting rotation of GameObject " + gameObject.name + " to: " + originalRotation);
    }

      public void ResetAllTransform()
    {
        // Log original transform values
        Debug.Log("Original Position: " + originalPosition);
        Debug.Log("Original Rotation: " + originalRotation);
        Debug.Log("Original Scale: " + originalScale);

        // Reset transforms
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        //transform.localScale = originalScale;

        Debug.Log("Transforms reset successfully.");
    }

}
 