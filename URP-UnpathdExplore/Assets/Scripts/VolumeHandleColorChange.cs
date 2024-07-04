using UnityEngine;
using UnityEngine.UI;

public class VolumeHandleColorChange : MonoBehaviour
{
    public Color greenColor = Color.green;
    public Color redColor = Color.red;

    private Image imageComponent;

    private void Start()
    {
        // Get the Image component attached to this GameObject
        imageComponent = GetComponent<Image>();

        if (SoundVolume.instance != null)
        {
            SoundVolume.instance.OnMuteStateChanged += UpdateHandleColor;
        }
        else
        {
            Debug.LogWarning("SoundVolume instance is not set.");
        }
    }

    public void SetColor(Color color)
    {
        // Check if the Image component is assigned
        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
        else
        {
            Debug.LogWarning("No Image component attached to this GameObject.");
        }
    }

    private void UpdateHandleColor(bool isMuted)
    {
        // Change the handle color based on the mute state
        SetColor(isMuted ? redColor : greenColor);
    }
}
