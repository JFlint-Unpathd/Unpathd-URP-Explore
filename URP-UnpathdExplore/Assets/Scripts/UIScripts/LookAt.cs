using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform label;
   // public Transform text;

   
    void Update()
    {
        // Find the main camera and set it as the target
        Transform target = Camera.main?.transform;

        if (target == null)
        {
            Debug.LogError("Main camera not found!");
            return; // exit the function if there is no main camera
        }

        // Set the LookAt rotation for the label and text
        if (label != null)
        {
            SetLookAt(label, target);
        }

        // if (text != null)
        // {
        //     SetLookAt(text, target);
        // }
    }

    // Function to set LookAt for a given transform based on a target
    void SetLookAt(Transform toRotate, Transform target)
    {
        Vector3 dirToTarget = (target.position - toRotate.position).normalized;
        toRotate.LookAt(toRotate.position - dirToTarget, Vector3.up);
    }
}
