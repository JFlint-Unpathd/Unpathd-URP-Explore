using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform label;
    public Transform text;

    // Start is called before the first frame update
    void Start()
    {
        // Find the main camera and set it as the target
        Transform target = Camera.main?.transform;

        if (target == null)
        {
            Debug.LogError("Main camera not found!");
        }

        // Set the target for the label and text
        if (label != null)
        {
            label.LookAt(target);
            // Optionally, you can set the local rotation to ensure correct orientation
            label.localRotation = Quaternion.Euler(0, 180, 0);
        }

        if (text != null)
        {
            text.LookAt(target);
            // Optionally, you can set the local rotation for the text as well
            text.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
