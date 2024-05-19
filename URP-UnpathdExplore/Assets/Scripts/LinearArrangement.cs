using UnityEngine;

public class LinearArrangement : MonoBehaviour
{
    // Array of objects to be arranged
    [SerializeField]
    private GameObject[] objects;

    // Distance between each object
    public float distance;

    void Start()
    {
        ArrangeObjectsInLine();
    }

    void ArrangeObjectsInLine()
    {
        // Get the parent's position
        Vector3 parentPosition = transform.position;

        for (int i = 0; i < objects.Length; i++)
        {
            // Set the position of each object relative to the parent's position
            objects[i].transform.position = parentPosition + new Vector3(i * distance, 0, 0);

            // Optionally, parent the object to this GameObject
            objects[i].transform.parent = transform;
        }
    }
}
