using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    private Vector3 originalPosition;

    void Start()
    {
        // Store the original position when the prefab is instantiated in the scene
        originalPosition = transform.position;
        //Debug.Log("Original Position Set to: " + originalPosition);
    }

    public void InstantiatePrefab()
    {
        // Instantiate the entire prefab (parent and children) at its original position
        GameObject instantiatedPrefab = Instantiate(gameObject, originalPosition, Quaternion.identity);

        // Make sure to set the instantiated prefab's position to its original position
        instantiatedPrefab.transform.position = originalPosition;

        // Log the positions
        //Debug.Log("Original Position: " + originalPosition + ", Instantiated Position: " + instantiatedPrefab.transform.position);
    }
}
