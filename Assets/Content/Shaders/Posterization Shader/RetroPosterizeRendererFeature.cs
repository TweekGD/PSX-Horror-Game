using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class RetroPosterizeRendererFeature : ScriptableRendererFeature
{
    private RetroPosterizePass pass;
    private Material material;

    public override void Create()
    {
        Shader shader = Shader.Find("Fullscreen/RetroPosterize");
        if (shader == null)
        {
            Debug.LogError("RetroPosterizeRendererFeature: shader 'Fullscreen/RetroPosterize' not found.");
            return;
        }

        material = CoreUtils.CreateEngineMaterial(shader);
        pass = new RetroPosterizePass(material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (pass == null || material == null)
            return;

        var stack = VolumeManager.instance.stack;
        var component = stack.GetComponent<RetroPosterizeVolumeComponent>();

        if (component == null || !component.IsActive())
            return;

        pass.Setup(component);
        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(material);
    }
}