using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ColorAdjustmentsManagerURP : MonoBehaviour
{
    private Volume volume;
    private ColorAdjustments colorAdjustments;

    [Header("UI Values")]
    [SerializeField] private TMP_Text postExposureValueText;
    [SerializeField] private TMP_Text contrastValueText;
    [SerializeField] private TMP_Text saturationValueText;
    [SerializeField] private TMP_Text hueShiftValueText;

    [Header("UI Dropdown")]
    [SerializeField] private TMP_Dropdown colorDropdown;
    List<string> colColorDropdown = new List<string> { "White", "Green", "Red", "Blue", "Black" };



    private void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out colorAdjustments);

    }

    public void ColorAdjOnOff(bool on)
    {
        if (on)
        {
            colorAdjustments.active = true;
        }
        else
        {
            colorAdjustments.active = false;
        }
    }


    public void SetPostExposure(float sliderValue)
    {
        colorAdjustments.postExposure.value = sliderValue;
        postExposureValueText.text = sliderValue.ToString("0");
    }
    public void SetContrast(float sliderValue)
    {
        colorAdjustments.postExposure.value = sliderValue;
        contrastValueText.text = sliderValue.ToString("0");
    }
    
    public void SetSaturation(float sliderValue)
    {
        colorAdjustments.saturation.value = sliderValue;
        saturationValueText.text = sliderValue.ToString("0");
    }
    
    public void SetHueShift(float sliderValue)
    {
        colorAdjustments.hueShift.value = sliderValue;
        hueShiftValueText.text = sliderValue.ToString("0");
        
    }

    public void ApplyColorFilter(int index)
    {
        switch (index)
        {
            case 0: colorAdjustments.colorFilter.value = Color.red; break;
            case 1: colorAdjustments.colorFilter.value = Color.black; break;
            case 2: colorAdjustments.colorFilter.value = Color.blue; break;
            case 3: colorAdjustments.colorFilter.value = Color.white; break;
            case 4: colorAdjustments.colorFilter.value = Color.green; break;
        }
    }

}
