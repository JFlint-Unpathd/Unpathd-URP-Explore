using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
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

        public Vector3 GetStoredPosition()
    {
        return originalPosition;
    }

    public Quaternion GetStoredRotation()
    {
        return originalRotation;
    }

    public void ResetRotation()
    {
        transform.rotation = originalRotation;
        Debug.Log("Resetting rotation of GameObject " + gameObject.name + " to: " + originalRotation);
    }

}
 