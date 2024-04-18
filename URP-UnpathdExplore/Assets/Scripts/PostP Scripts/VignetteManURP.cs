using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class VignetteManURP : MonoBehaviour
{
    [Header("Post Processing Volume")]
    private Volume _postProcessVolume;
    private UnityEngine.Rendering.Universal.Vignette _vignette;

    [Header("UI Dropdowns")]
    [SerializeField] private TMP_Dropdown vignetteColorDropdown;
    List<string> vignetteDropdownColor = new List<string> { "White", "Green", "Red", "Blue", "Black" };

    [Header("UI Values")]
    [SerializeField] private TMP_Text vignetteIntensityValueText;
    //[SerializeField] private TMP_Text vignetteSmoothnessValueText;



    private void Start()
    {
        _postProcessVolume = GetComponent<Volume>();
        _postProcessVolume.profile.TryGet(out _vignette);
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

    public void Center(float X, float Y) //(Default: X- 0.5 Y- 0.5)
    {
        _vignette.center.value = new Vector2(X, Y);
    }

    public void Intensity(float sliderValue) // (Default: 0)
    {
        _vignette.intensity.value = sliderValue;
        vignetteIntensityValueText.text = sliderValue.ToString("0");
    }

    public void Smoothness(float sliderValue) // (Default: 1)
    {
        _vignette.smoothness.value = sliderValue;
        //vignetteSmoothnessValueText.text = sliderValue.ToString("0");
    }

    public void SetColor(int index) // (Default: White)
    {      
        switch (index)
        {
            case 0: _vignette.color.value = Color.white; break;
            case 1: _vignette.color.value = Color.green; break;
            case 2: _vignette.color.value = Color.red; break;
            case 3: _vignette.color.value = Color.blue; break;
            case 4: _vignette.color.value = Color.black; break;
        }
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