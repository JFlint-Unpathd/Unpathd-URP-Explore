using UnityEngine;

[CreateAssetMenu(fileName = "ColorProperties", menuName = "ScriptableObjects/ColorProperties")]
public class ColorProperties : ScriptableObject
{
    public Color normalColor;
    public Color hoverColor;
    public Color selectedColor;
    public Color snappedColor;
}

