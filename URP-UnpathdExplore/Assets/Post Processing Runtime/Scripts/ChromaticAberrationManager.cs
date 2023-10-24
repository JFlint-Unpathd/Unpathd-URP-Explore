using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class ChromaticAberrationManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private ChromaticAberration _chromaticAb;

    [Header("Spectral LUT")]
    [SerializeField] private TextureParameter _spectralLutTex;

    [Header("UI Values")]
    [SerializeField] private Text chromaticIntensityValueText;

    void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _chromaticAb);
    }

    public void ChromaticAbberationOnOff(bool on)
    {
        if (on)
        {
            _chromaticAb.active = true;
        }
        else
        {
            _chromaticAb.active = false;
        }
    }

    public void ChromaticAberrationLUT() // (Default: None)
    {
        _chromaticAb.spectralLut = _spectralLutTex;
    }

    public void Intensity(float sliderValue) // 0 / 1 (Default: 0)
    {
        _chromaticAb.intensity.value = sliderValue;
    }

    public void SetFastMode(bool on) // (Default: Off)
    {
        if (on)
        {
            _chromaticAb.fastMode.value = true;
        }
        else
        {
            _chromaticAb.fastMode.value = false;    
        }
    }

}
