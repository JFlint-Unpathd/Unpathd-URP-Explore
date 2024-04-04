using UnityEngine;

public class CircleObjectPlacer : MonoBehaviour
{
    public float radius = 5f;

    void Awake()
    {
        ArrangeObjectsInCircle();
    }

    public void ArrangeObjectsInCircle()
    {
        int numberOfObjects = transform.childCount;

        float angleIncrement = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * angleIncrement * Mathf.Deg2Rad;
            Vector3 newPos = transform.position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

            // Set the position of the child object
            Transform childTransform = transform.GetChild(i);
            childTransform.position = newPos;

            // Calculate the direction vector towards the center
            Vector3 direction = Vector3.Normalize(Vector3.zero - newPos);

            // Rotate the object to face towards the center
            childTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Get the PrefabInstantiator script attached to the child prefab
            PrefabInstantiator prefabInstantiator = childTransform.GetComponent<PrefabInstantiator>();

            if (prefabInstantiator != null)
            {
                // Set the original position, rotation, and scale in the PrefabInstantiator
                // prefabInstantiator.OriginalPosition = newPos;
                // prefabInstantiator.OriginalRotation = childTransform.rotation;
                // prefabInstantiator.OriginalScale = childTransform.localScale;

                prefabInstantiator.SaveOriginalTransform();
            }
        }
    }
}
