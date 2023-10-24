using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class GrainManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private Grain _grain;

    [Header("UI Values")]
    [SerializeField] private Text grainIntensityValueText;
    [SerializeField] private Text grainSizeValueText;
    [SerializeField] private Text grainLuminanceValueText;

    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _grain);
    }

    public void GrainOnOff(bool on)
    {
        if (on)
        {
            _grain.active = true;
        }
        else
        {
            _grain.active = false;
        }
    }

    public void Colored(bool on) // (Default: True / On)
    {
        if(on)
        {
            _grain.colored.value = true;
        }
        else
        {
            _grain.colored.value = false;
        }
    }

    public void Intensity(float sliderValue) //0 - 1 (Default: 0)
    {
        _grain.intensity.value = sliderValue;
        grainIntensityValueText.text = sliderValue.ToString("0");
    }

    public void Size(float sliderValue) //0.3 - 3 (Default: 1)
    {
        _grain.size.value = sliderValue;
        grainSizeValueText.text = sliderValue.ToString("0");
    }

    public void LuminanceContribution(float sliderValue) //0 - 1 (Default: 0.8)
    {
        _grain.lumContrib.value = sliderValue;
        grainLuminanceValueText.text = sliderValue.ToString("0");
    }
}
