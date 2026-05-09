using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
[VolumeComponentMenu("Custom/Retro Posterize")]
[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
public class RetroPosterizeVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter posterizeLevels = new ClampedFloatParameter(4f, 1f, 32f, false);
    public ClampedFloatParameter ditherStrength = new ClampedFloatParameter(0.05f, 0f, 1f, false);
    public ClampedIntParameter pixelSize = new ClampedIntParameter(2, 1, 16, false);

    public bool IsActive() => posterizeLevels.value < 32f || ditherStrength.value > 0f || pixelSize.value > 1;
    public bool IsTileCompatible() => false;
}