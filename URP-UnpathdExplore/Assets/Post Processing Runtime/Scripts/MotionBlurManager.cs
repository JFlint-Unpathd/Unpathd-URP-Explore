using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class MotionBlurManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private MotionBlur _motionBlur;

    [Header("Values")]
    [SerializeField] private Text ShutterAngleText;
    [SerializeField] private Text SampleCountText;

    private void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _motionBlur);
    }

    public void MotionBlurOnOff(bool on)
    {
        if (on)
        {
            _motionBlur.active = true;
        }
        else
        {
            _motionBlur.active = false;
        }
    }

    public void ShutterAngle(float sliderValue) // 0 - 360 (Default: 270)
    {
        _motionBlur.shutterAngle.value = sliderValue;
        ShutterAngleText.text = sliderValue.ToString("0");
    }

    public void SampleCount(float sliderValue) // 4 - 32 (Default: 10)
    {
        _motionBlur.sampleCount.value = Mathf.FloorToInt(sliderValue);
        SampleCountText.text = sliderValue.ToString("0");
    }
}
