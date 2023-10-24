using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class AutoExposureManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private AutoExposure _autoExposure;

    [Header("UI Dropdowns")]
    [SerializeField] private Dropdown autoExposureTypeDropdown;
    List<string> autoExposureType = new List<string> { "Progressive", "Fixed" };

    [Header("UI Values")]
    [SerializeField] private Text minimumExpValueText;
    [SerializeField] private Text maximumExpValueText;
    [SerializeField] private Text expCompValueText;
    [SerializeField] private Text speedUpValueText;
    [SerializeField] private Text speedDownValueText;

    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _autoExposure);
    }

    public void AutoExposureOnOff(bool on)
    {
        if (on)
        {
            _autoExposure.active = true;
        }
        else
        {
            _autoExposure.active = false;
        }
    }

    public void ExposureMinimum(float sliderValue) // -9 / 9 (Default: 0)
    {
        _autoExposure.minLuminance.value = sliderValue;
        minimumExpValueText.text = sliderValue.ToString("0");
    }

    public void ExposureMaximum(float sliderValue) // -9 / 9 (Default: 0)
    {
        _autoExposure.maxLuminance.value = sliderValue;
        maximumExpValueText.text = sliderValue.ToString("0");
    }

    public void ExposureCompensation(float sliderValue) // (Default: 1)
    {
        _autoExposure.keyValue.value = sliderValue;
        expCompValueText.text = sliderValue.ToString("0");
    }

    public void SetAdaptionType(int index) // (Default: Progressive)
    {      
        switch (index)
        {
            case 0: _autoExposure.eyeAdaptation.value = EyeAdaptation.Progressive; break;
            case 1: _autoExposure.eyeAdaptation.value = EyeAdaptation.Fixed; break;
        }
    }

    public void ExposureSpeedUp(float sliderValue) // (Default: 2)
    {
        _autoExposure.speedUp.value = sliderValue;
        speedUpValueText.text = sliderValue.ToString("0");
    }

    public void ExposureSpeedDown(float sliderValue) // (Default: 1)
    {
        _autoExposure.speedDown.value = sliderValue;
        speedDownValueText.text = sliderValue.ToString("0");
    }
}
