using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularArrangement : MonoBehaviour
{
    public GameObject[] items; // Input items in the inspector
    public float radius = 5f;  // Adjust the radius to set the spacing

    void Awake()
    {
        //ArrangeItems();
        ArrangeObjectsInCircle();
    }


    void ArrangeItems()
    {
        float angleStep = 2 * Mathf.PI / items.Length;

        for (int i = 0; i < items.Length; i++)
        {
            float posX = Mathf.Cos(i * angleStep) * radius;
            float posY = Mathf.Sin(i * angleStep) * radius;
            items[i].transform.position = new Vector3(posX, posY, 0) + transform.position;
            
            items[i].transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void ArrangeObjectsInCircle()
    {
        int numberOfObjects = items.Length;

        if (numberOfObjects == 0)
        {
            Debug.LogWarning("No items to arrange.");
            return;
        }

        float angleIncrement = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * angleIncrement * Mathf.Deg2Rad;
            Vector3 newPos = transform.position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

            // Set the position of the object
            items[i].transform.position = newPos;

            // Calculate the direction vector towards the center
            Vector3 direction = transform.position - newPos;

            // Rotate the object to face towards the center
            items[i].transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            // If the item has a TransformKeeper component, save the original transform
            TransformKeeper transformKeeper = items[i].GetComponent<TransformKeeper>();
            if (transformKeeper != null)
            {
                transformKeeper.SaveOriginalTransform();
            }
        }
    }

}

