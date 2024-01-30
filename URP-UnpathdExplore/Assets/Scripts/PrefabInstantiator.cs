using UnityEngine;

public class PrefabInstantiator  : MonoBehaviour
{
    private Vector3 originalPosition;

    public void InstantiatePrefab()
    {
        // Instantiate the entire prefab (parent and children) at its original position
        GameObject instantiatedPrefab = Instantiate(gameObject, originalPosition, Quaternion.identity);
        Debug.Log("Original Position: " + originalPosition + ", Instantiated Position: " + instantiatedPrefab.transform.position);
    }

}
