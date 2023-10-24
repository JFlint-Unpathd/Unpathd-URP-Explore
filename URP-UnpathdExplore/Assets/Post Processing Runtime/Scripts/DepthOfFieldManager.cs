using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class DepthOfFieldManager : MonoBehaviour
{
    private PostProcessVolume _postProcessVolume;
    private DepthOfField _dof;

    [Header("Dropdowns")]
    [SerializeField] private Dropdown maxBlurSizeDropdown;
    List<string> maxBlurSizeList = new List<string> { "Small", "Medium", "Large", "Very Large" };

    [Header("UI Values")]
    [SerializeField] private Text dofFocusDistanceValueText;
    [SerializeField] private Text dofApertureValueText;
    [SerializeField] private Text dofFocalLengthValueText;

    void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _dof);
    }

    public void DOFOnOff(bool on)
    {
        if (on)
        {
            _dof.active = true;
        }
        else
        {
            _dof.active = false;
        }
    }

    public void FocusDistance(float sliderValue) //10 Default
    {
        _dof.focusDistance.value = sliderValue;
        dofFocusDistanceValueText.text = sliderValue.ToString("0");
    }

    public void Aperture(float sliderValue) //0.1 - 32 (Default 5.6)
    {
        _dof.aperture.value = sliderValue;
        dofApertureValueText.text = sliderValue.ToString("0");
    }

    public void FocalLength(float sliderValue) //1 - 300 (Default 50)
    {
        _dof.focalLength.value = sliderValue;
        dofFocalLengthValueText.text = sliderValue.ToString("0");
    }

    public void MaxBlurSize(int index) //(Default: Medium)
    {
        switch(index)
        {
            case 0: _dof.kernelSize.value = KernelSize.Small; break;
            case 1: _dof.kernelSize.value = KernelSize.Medium; break;
            case 2: _dof.kernelSize.value = KernelSize.Large; break;
            case 3: _dof.kernelSize.value = KernelSize.VeryLarge; break;
        }
    }
}
