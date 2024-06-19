using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Implement the ITunnelingVignetteProvider interface
public class SceneLoadingVignetteProvider : MonoBehaviour, ITunnelingVignetteProvider
{
    [SerializeField]
    private VignetteParameters m_Parameters = new VignetteParameters();

    // Getter for ITunnelingVignetteProvider interface
    public VignetteParameters vignetteParameters => m_Parameters;

    // Example method to update vignette parameters based on loading progress
    public void UpdateVignetteParameters(float progress)
    {
        // Example: Adjust vignette aperture size based on loading progress
        m_Parameters.apertureSize = Mathf.Lerp(0.5f, 0.2f, progress);
        
        // Update other parameters as needed based on your logic
    }

    private void Start()
    {
        
    }
    private void Awake()
    {
        // Customize default vignette parameters for scene loading
        m_Parameters.apertureSize = 0.5f; // Example value
        m_Parameters.featheringEffect = 0.3f; // Example value
        m_Parameters.easeInTime = 1.0f; // Example value
        m_Parameters.easeOutTime = 1.0f; // Example value
        m_Parameters.vignetteColor = Color.black; // Example value
        m_Parameters.vignetteColorBlend = Color.black; // Example value
    }
}
