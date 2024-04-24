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
        // Get the Image component attached to this GameObject
        imageComponent = GetComponent<Image>();

        // Set the initial color to green
        imageComponent.color = greenColor;
    }

    public void SetColor(Color color)
    {
        imageComponent.color = color;
    }
    
    
}
