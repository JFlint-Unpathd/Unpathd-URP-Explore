using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class VignetteManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private Vignette _vignette;

    [Header("Color Filter")]
    [SerializeField] private Dropdown colorDropdown;
    List<string> colorList = new List<string> { "White", "Red", "Green", "Blue", "Black" };

    [Header("Values")]
    [SerializeField] private Text intensityValueText;
    [SerializeField] private Text SmoothnessValueText;
    [SerializeField] private Text RoundnessValueText;

    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _vignette);
    }

    public void VignetteOnOff(bool on)
    {
        if (on)
        {
            _vignette.active = true;
        }
        else
        {
            _vignette.active = false;
        }
    }

    public void VignetteColor(int index) //Black
    {
        switch (index)
        {
            case 0: _vignette.color.value = Color.white; break;
            case 1: _vignette.color.value = Color.red; break;
            case 2: _vignette.color.value = Color.green; break;
            case 3: _vignette.color.value = Color.blue; break;
            case 4: _vignette.color.value = Color.black; break;
        }
    }

    public void Center(float X, float Y) //(Default: X- 0.5 Y- 0.5)
    {
        _vignette.center.value = new Vector2(X, Y);
    }

    public void Intensity(float sliderValue) // 0 - 1 (Default:  0)
    {
        _vignette.intensity.value = sliderValue;
        intensityValueText.text = sliderValue.ToString("0");
    }

    public void Smoothness(float sliderValue) // 0.01 - 1 (Default: 0.2)
    {
        _vignette.smoothness.value = sliderValue;
        SmoothnessValueText.text = sliderValue.ToString("0");
    }

    public void Roundness(float sliderValue) //0 - 1 (Default: 1)
    {
        _vignette.roundness.value = sliderValue;
        RoundnessValueText.text = sliderValue.ToString("0");
    }

    public void Rounded(bool on) //false
    {
        if(on)
        {
            _vignette.rounded.value = true;
        }
        else
        {
            _vignette.rounded.value = false;
        }
    }
}
