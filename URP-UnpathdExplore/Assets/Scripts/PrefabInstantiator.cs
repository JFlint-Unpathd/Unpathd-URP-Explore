// 2024-03-26 AI-Tag 
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        Debug.Log(gameObject.name + " Original Position Set in Awake: " + originalPosition);
        Debug.Log(gameObject.name + " Original Rotation Set in Awake: " + originalRotation);
    }

    public void InstantiatePrefab()
    {
        // Instantiate the entire prefab (parent and children) at its original position
        GameObject instantiatedPrefab = Instantiate(gameObject, originalPosition, originalRotation);

        Debug.Log(gameObject.name + " Original Position in InstantiatePrefab: " + originalPosition);
        Debug.Log(gameObject.name + " Original Rotation in InstantiatePrefab: " + originalRotation);
        Debug.Log(gameObject.name + " Instantiated Position in InstantiatePrefab: " + instantiatedPrefab.transform.position);
    }

    // Add methods to retrieve the original position and rotation.
    public Vector3 GetOriginalPosition()
    {
        Debug.Log(gameObject.name + " GetOriginalPosition in PrefabInstantiator: " + originalPosition);
        return originalPosition;
    }

    public Quaternion GetOriginalRotation()
    {
        Debug.Log(gameObject.name + " GetOriginalRotation in PrefabInstantiator: " + originalRotation);
        return originalRotation;
    }
}
