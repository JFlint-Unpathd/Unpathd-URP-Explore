using UnityEngine;

public class LinearArrangement : MonoBehaviour
{
    // Array of prefabs for the objects to be arranged
    [SerializeField]
    private GameObject[] objectPrefabs;
  
    // Distance between each object
    public float distance;

    void Start()
    {
        ArrangeObjectsInLine();
    }

    void ArrangeObjectsInLine()
    {
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            // Select prefab randomly from objectPrefabs array
            GameObject prefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];

            // Instantiate the selected object prefab
            GameObject obj = Instantiate(prefab, new Vector3(i * distance, 0, 0), Quaternion.identity);

            // Parent the object to this GameObject
            obj.transform.parent = transform;
        }
    }
}
