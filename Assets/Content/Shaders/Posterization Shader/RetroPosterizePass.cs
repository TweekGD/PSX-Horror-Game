using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class RetroPosterizePass : ScriptableRenderPass
{
    private static readonly int PosterizeLevelsId = Shader.PropertyToID("_PosterizeLevels");
    private static readonly int DitherStrengthId = Shader.PropertyToID("_DitherStrength");
    private static readonly int PixelSizeId = Shader.PropertyToID("_PixelSize");

    private readonly Material material;

    private class PassData
    {
        internal TextureHandle source;
        internal Material material;
    }

    public RetroPosterizePass(Material mat)
    {
        material = mat;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void Setup(RetroPosterizeVolumeComponent component)
    {
        material.SetFloat(PosterizeLevelsId, component.posterizeLevels.value);
        material.SetFloat(DitherStrengthId, component.ditherStrength.value);
        material.SetInt(PixelSizeId, component.pixelSize.value);
    }

    private static void ExecutePass(PassData data, RasterGraphContext context)
    {
        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1f, 1f, 0f, 0f), data.material, 0);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        if (cameraData.isSceneViewCamera)
            return;

        TextureHandle cameraColor = resourceData.cameraColor;
        if (!cameraColor.IsValid())
            return;

        var desc = renderGraph.GetTextureDesc(cameraColor);
        desc.name = "_RetroPosterizeTemp";
        desc.clearBuffer = false;
        TextureHandle destination = renderGraph.CreateTexture(desc);

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("Retro Posterize Pass", out var passData))
        {
            passData.source = cameraColor;
            passData.material = material;

            builder.UseTexture(passData.source);
            builder.SetRenderAttachment(destination, 0);
            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) => ExecutePass(data, ctx));
        }

        resourceData.cameraColor = destination;
    }
}