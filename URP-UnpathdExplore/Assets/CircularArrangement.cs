using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularArrangement : MonoBehaviour
{
    public GameObject[] items; // Input items in the inspector
    public float radius = 5f;  // Adjust the radius to set the spacing

    void Start()
    {
        ArrangeItems();
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



}

