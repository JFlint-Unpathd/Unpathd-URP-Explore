using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class PostProcessing : MonoBehaviour
{
    [Header("Post Processing Volume")]
    [SerializeField] private Volume postProcessing;
    [SerializeField] private bool disable;

    [Header("Post Processing Profiles")]
    [SerializeField] private VolumeProfile postProfileMain;
    [SerializeField] private VolumeProfile postProfileSecondary;

    [Header("Post Processing Effects")]
    private Bloom bloom;

    private void Start()
    {
        postProcessing.profile.TryGet(out bloom);
    }

    public void MainPostProcess()
    {
        postProcessing.profile = postProfileMain;
    }

    public void SecondaryPostProcess()
    {
        postProcessing.profile = postProfileSecondary;
    }

    public void DisablePostProcess()
    {
        disable = !disable;
        postProcessing.enabled = disable;
    }

    public void AdjustBloom()
    {
        bloom.intensity.value = 10f;
    }

}
