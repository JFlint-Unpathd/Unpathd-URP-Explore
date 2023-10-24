using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class BloomManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private Bloom _bloom;

    [Header("UI Dropdowns")]
    [SerializeField] private Dropdown bloomColorDropdown;
    List<string> bloomDropdownColor = new List<string> { "White", "Green", "Red", "Blue", "Black" };

    [Header("Dirt Texture")]
    [SerializeField] private TextureParameter customDirtTexture;

    [Header("UI Values")]
    [SerializeField] private Text bloomIntensityValueText;
    [SerializeField] private Text bloomThresholdValueText;
    [SerializeField] private Text bloomSoftKneeValueText;
    [SerializeField] private Text bloomDiffusionValueText;
    [SerializeField] private Text bloomAnamorphValueText;

    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _bloom);
    }

    public void BloomOnOff(bool on)
    {
        if (on)
        {
            _bloom.active = true;
        }
        else
        {
            _bloom.active = false;
        }
    }

    public void Intensity(float sliderValue) // (Default: 0)
    {
        _bloom.intensity.value = sliderValue;
        bloomIntensityValueText.text = sliderValue.ToString("0");
    }

    public void Threshold(float sliderValue) // (Default: 1)
    {
        _bloom.threshold.value = sliderValue;
        bloomThresholdValueText.text = sliderValue.ToString("0");
    }

    public void SoftKnee(float sliderValue) // 0 - 1 (Default: 0.5)
    {
        _bloom.softKnee.value = sliderValue;
        bloomSoftKneeValueText.text = sliderValue.ToString("0");
    }

    public void Clamp(float sliderValue) // (Default: 65472)
    {
        _bloom.clamp.value = sliderValue;
    }

    public void Diffusion(float sliderValue) // 1 - 10 (Default: 7)
    {
        _bloom.diffusion.value = sliderValue;
        bloomDiffusionValueText.text = sliderValue.ToString("0");
    }

    public void AnamorphicRatio(float sliderValue) // -1 - 1 (Default: 0)
    {
        _bloom.anamorphicRatio.value = sliderValue;
        bloomAnamorphValueText.text = sliderValue.ToString("0");
    }

    public void SetColor(int index) // (Default: White)
    {      
        switch (index)
        {
            case 0: _bloom.color.value = Color.white; break;
            case 1: _bloom.color.value = Color.green; break;
            case 2: _bloom.color.value = Color.red; break;
            case 3: _bloom.color.value = Color.blue; break;
            case 4: _bloom.color.value = Color.black; break;
        }
    }

    public void SetFastMode(bool FastModeOn) // (Default: Off)
    {
        if (FastModeOn)
        {
            _bloom.fastMode.value = true;
        }
        else
        {
            _bloom.fastMode.value = false;
        }
    }

    public void DirtTexture() // (Default: Empty)
    {
        _bloom.dirtTexture = customDirtTexture;
    }

    public void DirtIntensity(float sliderValue) // (Default: 0)
    {
        _bloom.dirtIntensity.value = sliderValue;
    }
}
