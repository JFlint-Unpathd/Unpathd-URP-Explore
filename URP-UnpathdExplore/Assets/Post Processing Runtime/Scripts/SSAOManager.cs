using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class SSAOManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private ScreenSpaceReflections _SSAO;

    [SerializeField] private Dropdown ssaoDropdown;
    private List<string> ssaoPresetList = new List<string> { "Lower", "Low", "Medium", "High", "Higher", "Ultra", "Overkill" };

    [Header("Values")]
    [SerializeField] private Text maxMarchDistValueText;
    [SerializeField] private Text distFadeValueText;
    [SerializeField] private Text vignetteValueText;

    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _SSAO);
    }

    public void SSAOOnOff(bool on)
    {
        if (on)
        {
            _SSAO.active = true;
        }
        else
        {
            _SSAO.active = false;
        }
    }

    public void SSAOPreset(int index)  //Medium
    {
        switch(index)
        {
            case 0: _SSAO.preset.value = ScreenSpaceReflectionPreset.Lower; break;
            case 1: _SSAO.preset.value = ScreenSpaceReflectionPreset.Low; break;
            case 2: _SSAO.preset.value = ScreenSpaceReflectionPreset.Medium; break;
            case 3: _SSAO.preset.value = ScreenSpaceReflectionPreset.High; break;
            case 4: _SSAO.preset.value = ScreenSpaceReflectionPreset.Higher; break;
            case 5: _SSAO.preset.value = ScreenSpaceReflectionPreset.Ultra; break;
            case 6: _SSAO.preset.value = ScreenSpaceReflectionPreset.Overkill; break;
        }
    }

    public void MaximumMarchDistance(float sliderValue) // 0 - 300 (Default: 100)
    {
        _SSAO.maximumMarchDistance.value = sliderValue;
        maxMarchDistValueText.text = sliderValue.ToString("0");
    }

    public void DistanceFade(float sliderValue) // 0 - 1 (Default 0.5)
    {
        _SSAO.distanceFade.value = sliderValue;
        distFadeValueText.text = sliderValue.ToString("0");
    }

    public void Vignette(float sliderValue) // 0 - 1 (Default 0.5)
    {
        _SSAO.vignette.value = sliderValue;
        vignetteValueText.text = sliderValue.ToString("0");
    }
}
