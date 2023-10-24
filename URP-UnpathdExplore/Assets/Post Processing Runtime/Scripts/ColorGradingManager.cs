using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class ColorGradingManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private ColorGrading _colorGrading;

    [Header("Color Mode")]
    [SerializeField] private Dropdown colorModeDropdown;
    List<string> colorModeList = new List<string> { "LDR", "HDR" };

    [Header("Tonemapping Mode")]
    [SerializeField] private Dropdown tonemappingModeDropdown;
    List<string> tonemappingModeList = new List<string> { "None", "Neutral", "ACES" };

    [Header("Color Filter")]
    [SerializeField] private Dropdown colorFilterDropdown;
    List<string> colorFilterList = new List<string> { "White", "Red", "Green", "Blue", "Black" };

    [Header("UI Values")]
    [SerializeField] private Text colorGradeTempValueText;
    [SerializeField] private Text colorGradeTintValueText;
    [SerializeField] private Text colorGradePostExValueText;
    [SerializeField] private Text colorGradeHueValueText;
    [SerializeField] private Text colorGradeSaturationValueText;
    [SerializeField] private Text colorGradeContrastValueText;

    void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _colorGrading);
    }

    public void ColorGradingOnOff(bool on)
    {
        if (on)
        {
            _colorGrading.active = true;
        }
        else
        {
            _colorGrading.active = false;
        }
    }

    public void MainPresetMode(int index) // (Default: HDR)
    {
        switch(index)
        {
            case 0: _colorGrading.gradingMode.value = GradingMode.LowDefinitionRange; break;
            case 1: _colorGrading.gradingMode.value = GradingMode.HighDefinitionRange; break;
        }
    }

    public void TonemappingMode(int index) // (Default: None)
    {
        switch(index)
        {
            case 0: _colorGrading.tonemapper.value = Tonemapper.None; break;
            case 1: _colorGrading.tonemapper.value = Tonemapper.Neutral; break;
            case 2: _colorGrading.tonemapper.value = Tonemapper.ACES; break;
        }
    }

    //White Balance
    public void Temperature(float sliderValue) //-100 / 100 (Default: 0)
    {
        _colorGrading.temperature.value = sliderValue;
        colorGradeTempValueText.text = sliderValue.ToString("0");
    }

    public void Tint(float sliderValue) //-100 / 100 (Default: 0)
    {
        _colorGrading.tint.value = sliderValue;
        colorGradeTintValueText.text = sliderValue.ToString("0");
    }

    //Tone
    public void PostExposure(float sliderValue) //-100 - 100 (Default: 0) 
    {
        _colorGrading.postExposure.value = sliderValue;
        colorGradePostExValueText.text = sliderValue.ToString("0");
    }

    public void ColorFilter(int index) // (Default: White)
    {
        switch (index)
        {
            case 0: _colorGrading.colorFilter.value = Color.white; break;
            case 1: _colorGrading.colorFilter.value = Color.red; break;
            case 2: _colorGrading.colorFilter.value = Color.green; break;
            case 3: _colorGrading.colorFilter.value = Color.blue; break;
            case 4: _colorGrading.colorFilter.value = Color.black; break;
        }
    }

    public void HueShift(float sliderValue) // -180 / 180 (Default: 0)
    {
        _colorGrading.hueShift.value = sliderValue;
        colorGradeHueValueText.text = sliderValue.ToString("0");
    }

    public void Saturation(float sliderValue) // -100 / 100 (Default: 0)
    {
        _colorGrading.saturation.value = sliderValue;
        colorGradeSaturationValueText.text = sliderValue.ToString("0");
    }

    public void Contrast(float sliderValue) // -100 / 100 (Default: 0)
    {
        _colorGrading.contrast.value = sliderValue;
        colorGradeContrastValueText.text = sliderValue.ToString("0");
    }
}
