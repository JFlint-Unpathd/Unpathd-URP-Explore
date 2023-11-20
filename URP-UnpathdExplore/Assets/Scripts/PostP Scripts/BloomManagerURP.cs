using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections.Generic;

public class BloomManagerURP : MonoBehaviour
{
    private Volume _volume;
    private Bloom _bloom;

    [Header("UI Dropdowns")]
    [SerializeField] private Dropdown bloomColorDropdown;
    List<string> bloomDropdownColor = new List<string> { "White", "Green", "Red", "Blue", "Black" };

    [Header("UI Values")]
    [SerializeField] private Text bloomIntensityValueText;
    [SerializeField] private Text bloomThresholdValueText;
    

    private void Start()
    {
        _volume = GetComponent<Volume>();
        _volume.profile.TryGet(out _bloom);

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

    public void SetIntensity(float sliderValue)
    {
         _bloom.intensity.value = sliderValue;
        bloomIntensityValueText.text = sliderValue.ToString("0");
    }

    public void SetThreshold(float sliderValue)
    {
       _bloom.threshold.value = sliderValue;
        bloomThresholdValueText.text = sliderValue.ToString("0");
    }

    public void SetColor(int index)
    {
        switch (index)
        {
            case 0: _bloom.tint.value = Color.white; break;
            case 1: _bloom.tint.value = Color.green; break;
            case 2: _bloom.tint.value = Color.red; break;
            case 3: _bloom.tint.value = Color.blue; break;
            case 4: _bloom.tint.value = Color.black; break;
        }
    }
}
