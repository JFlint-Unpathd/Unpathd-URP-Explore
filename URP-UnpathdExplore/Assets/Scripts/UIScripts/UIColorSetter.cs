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
                //Debug.Log("Setting text color to: " + UIColorManager.Instance.CurrentColors.textColor);
                textComponent.color = UIColorManager.Instance.CurrentColors.textColor;
                //Debug.Log("color set");
            }

            if (imageComponent != null)
            {
                //Debug.Log("Setting panel color to: " + UIColorManager.Instance.CurrentColors.panelColor);
                imageComponent.color = UIColorManager.Instance.CurrentColors.panelColor;
                //Debug.Log("color set");
            }

            var slider = GetComponent<Slider>();
            if (slider != null)
            {
                slider.targetGraphic.color = UIColorManager.Instance.CurrentColors.sliderColor;
                var handleSprite = slider.handleRect.GetComponent<Image>();
                if (handleSprite != null)
                {
                    handleSprite.color = UIColorManager.Instance.CurrentColors.handleColor;
                }
            }

            var image = GetComponent<Image>();
            if (image != null && gameObject.tag == "Line")
            {
                image.color = UIColorManager.Instance.CurrentColors.lineColor;
            }
            
            if (image != null && gameObject.tag == "Outer")
            {
                image.color = UIColorManager.Instance.CurrentColors.outerColor;
            }
            
    }

}
}
