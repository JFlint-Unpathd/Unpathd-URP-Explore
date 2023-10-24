using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class AmbientOcclusionManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private AmbientOcclusion _ao;

    [Header("UI Dropdowns")]
    [SerializeField] private Dropdown ambientColorDropdown;
    List<string> ambientDropdownColor = new List<string> { "Black", "Green", "Red", "Blue" };

    [SerializeField] private Dropdown ambientQualityDropdown;
    List<string> ambientDropdownQuality = new List<string> { "Lowest", "Low", "Medium", "High", "Ultra" };

    [Header("UI Values")]
    [SerializeField] private Text aoIntensityValueText;
    [SerializeField] private Text aoRadiusValueText;

    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _ao);
    }

    public void AmbientOcclusionOnOff(bool on)
    {
        if (on)
        {
            _ao.active = true;
        }
        else
        {
            _ao.active = false;
        }
    }

    public void AmbientMode(int index) // (Default: MultiScaleVolumetricObscurance)
    {
        switch(index)
        {
            case 0: _ao.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance; break;
            case 1: _ao.mode.value = AmbientOcclusionMode.MultiScaleVolumetricObscurance; break;
        }
    }

    public void AmbientOcclusionIntensity(float sliderValue) // 0 - 4 (Default: 0)
    {
        _ao.intensity.value = sliderValue;
        aoIntensityValueText.text = sliderValue.ToString("0");
    }

    public void AmbientThickness(float sliderValue)
    {
        _ao.thicknessModifier.value = sliderValue;
        //Update UI if you wish
    }

    public void AmbientOcclusionRadius(float sliderValue) // (Default: 0.25)
    {
        _ao.radius.value = sliderValue;
        aoRadiusValueText.text = sliderValue.ToString("0");
    }

    public void SetAmbientOcclusionQuality(int index) // (Default: Medium)
    {      
        switch (index)
        {
            case 0: _ao.quality.value = AmbientOcclusionQuality.Lowest; break;
            case 1: _ao.quality.value = AmbientOcclusionQuality.Low; break;
            case 2: _ao.quality.value = AmbientOcclusionQuality.Medium; break;
            case 3: _ao.quality.value = AmbientOcclusionQuality.High; break;
            case 4: _ao.quality.value = AmbientOcclusionQuality.Ultra; break;
        }
    }

    public void AmbientOcclusionColor(int index) // (Default: Black)
    {
        switch (index)
        {
            case 0: _ao.color.value = Color.black; break;
            case 1: _ao.color.value = Color.green; break;
            case 2: _ao.color.value = Color.red; break;
            case 3: _ao.color.value = Color.blue; break;
        }
    }

    public void AmbientOcclusionOnly(bool ambientParaOnly) // (Default: On / True)
    {
        if (ambientParaOnly)
        {
            _ao.ambientOnly.value = true;
        }
        else
        {
            _ao.ambientOnly.value = false;
        }
    }
}
