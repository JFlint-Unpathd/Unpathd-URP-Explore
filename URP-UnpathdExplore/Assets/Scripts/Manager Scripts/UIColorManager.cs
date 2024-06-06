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
    public Color sliderColor;
    public Color handleColor;
    public Color lineColor;
    public Color outerColor;
    public Color checkBoxColor;
    public Color checkMarkColor;
    public Color dropDownColor;
    
}


public class UIColorManager : MonoBehaviour
{
    public static UIColorManager Instance; // Singleton instance

    [SerializeField] private UIColorPair[] colorPairs; // Array of color pairs for each button
    public UIColorPair CurrentColors { get; private set; } // Current color pair

    private void Awake()
    {

        // Setup singleton instance
        if (Instance == null)
        {
            Instance = this;
            PersistanceClass.DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Set a default color pair
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
