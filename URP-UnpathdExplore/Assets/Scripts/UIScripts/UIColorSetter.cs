// 2024-03-22 AI-Tag 
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIColorSetter : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private Image imageComponent;
    
    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        imageComponent = GetComponent<Image>();
    }

    private void OnEnable()
    {
        SetColor();
    }

    public void SetColor()
    {
        if (UIColorManager.Instance != null)
        {
            if (textComponent != null)
            {
                textComponent.color = UIColorManager.Instance.CurrentColors.textColor;
            }

            if (imageComponent != null)
            {
                switch (gameObject.tag)
                {
                    case "Panel":
                        imageComponent.color = UIColorManager.Instance.CurrentColors.panelColor;
                        break;
                    case "Line":
                        imageComponent.color = UIColorManager.Instance.CurrentColors.lineColor;
                        break;
                    case "Outer":
                        imageComponent.color = UIColorManager.Instance.CurrentColors.outerColor;
                        break;
                    case "Slider":
                        imageComponent.color = UIColorManager.Instance.CurrentColors.sliderColor;
                        break;
                    case "Handle":
                        imageComponent.color = UIColorManager.Instance.CurrentColors.handleColor;
                        break;
                    case "CheckBox":
                        imageComponent.color = UIColorManager.Instance.CurrentColors.checkBoxColor;
                        break;
                    case "CheckMark":
                        imageComponent.color = UIColorManager.Instance.CurrentColors.checkMarkColor;
                        break;

                }
            }
        }
    }
}
