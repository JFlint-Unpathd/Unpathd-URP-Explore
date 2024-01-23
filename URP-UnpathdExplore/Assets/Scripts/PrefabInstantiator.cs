using UnityEngine;

public class PrefabInstantiator  : MonoBehaviour
{
    private Vector3 originalPosition;

    void Awake()
    {
        // Store the original position when the prefab is instantiated in the scene
        originalPosition = transform.position;
    }

    public void InstantiatePrefab()
    {
        // Instantiate the entire prefab (parent and children) at its original position
        GameObject instantiatedPrefab = Instantiate(gameObject, originalPosition, Quaternion.identity);
        
    }
}
