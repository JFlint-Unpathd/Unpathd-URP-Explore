// 2024-06-05 AI-Tag 
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeHandleColorChange : MonoBehaviour
{
    public Color greenColor = Color.green;
    public Color redColor = Color.red;

    private Image imageComponent;
    private bool isGreen = true;

    private void Start()
    {
        // Set the initial color to green
        SetColor(greenColor);
    }

    public void SetColor(Color color)
    {
        // Get the Image component attached to this GameObject
        imageComponent = GetComponent<Image>();

        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
        else
        {
            Debug.Log("No Image component attached to this GameObject.");
        }
    }
}
