using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public struct UIColorPair
{
    public Color textColor;
    public Color panelColor;
}


public class UIColorManager : MonoBehaviour
{
    public static UIColorManager Instance; // Singleton instance

    [SerializeField] private UIColorPair[] colorPairs; // Array of color pairs for each button
    public UIColorPair CurrentColors { get; private set; } // Current color pair

    private void Awake()
    {
        Debug.Log("UIColorManager Initialized");

        // Setup singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Set a default color pair, do it here
        // Use the first color pair by default
        CurrentColors = colorPairs[2];
    }

    public void ChangeColor(int index)
    {
        
        CurrentColors = colorPairs[index];
        Debug.Log("new pair assigned");

        // Find all UIColorSetters and update their color
        foreach (var colorSetter in FindObjectsOfType<UIColorSetter>())
        {
            colorSetter.SetColor();
        }
    }
}
