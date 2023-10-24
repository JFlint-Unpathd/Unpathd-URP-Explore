using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class LensDistortionManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private LensDistortion _lensDistort;

    [Header("Values")]
    [SerializeField] private Text intensityValueText;
    [SerializeField] private Text xMultiValueText;
    [SerializeField] private Text yMultiValueText;
    [SerializeField] private Text centerXValueText;
    [SerializeField] private Text centerYValueText;
    [SerializeField] private Text scaleValueText;

    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _lensDistort);
    }

    public void LensDistortOnOff(bool on)
    {
        if (on)
        {
            _lensDistort.active = true;
        }
        else
        {
            _lensDistort.active = false;
        }
    }

    public void Intensity(float sliderValue) //-100 / 100 (Default: 0)
    {
        _lensDistort.intensity.value = sliderValue;
        intensityValueText.text = sliderValue.ToString("0");
    }

    public void XMultiplier(float sliderValue) // 0 - 1 (Default: 1)
    {
        _lensDistort.intensityX.value = sliderValue;
        xMultiValueText.text = sliderValue.ToString("0");
    }

    public void YMultiplier(float sliderValue) //0 - 1 (Default: 1)
    {
        _lensDistort.intensityY.value = sliderValue;
        yMultiValueText.text = sliderValue.ToString("0");
    }

    public void CenterX(float sliderValue) //-1 - 1 (Default: 0)
    {
        _lensDistort.centerX.value = sliderValue;
        centerXValueText.text = sliderValue.ToString("0");
    }

    public void CenterY(float sliderValue) //-1 - 1 (Default: 0)
    {
        _lensDistort.centerY.value = sliderValue;
        centerYValueText.text = sliderValue.ToString("0");
    }

    public void Scale(float sliderValue) //0.05 - 5 (Default: 1)
    {
        _lensDistort.scale.value = sliderValue;
        scaleValueText.text = sliderValue.ToString("0");
    }
}
