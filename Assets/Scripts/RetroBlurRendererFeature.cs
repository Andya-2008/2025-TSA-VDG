using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RetroBlurRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class RetroBlurSettings
    {
        public Material blurMaterial;
        [Range(0f, 10f)]
        public float blurStrength = 0f;
    }

    public RetroBlurSettings settings = new RetroBlurSettings();

    class RetroBlurPass : ScriptableRenderPass
    {
        private Material blurMaterial;
        private float blurStrength;

        private RTHandle tempTexture;
        private RTHandle source;

        public RetroBlurPass(Material blurMat)
        {
            blurMaterial = blurMat;
        }

        public void SetBlur(float strength)
        {
            blurStrength = strength;
        }

        public void Setup(RTHandle src)
        {
            source = src;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Allocate a temporary RTHandle using the camera descriptor
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, descriptor, name: "_RetroBlurTempTex");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (blurMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("RetroBlur");

            blurMaterial.SetFloat("_BlurSize", blurStrength);

            // Blit ? Blur ? Blit back
            Blitter.BlitCameraTexture(cmd, source, tempTexture, blurMaterial, 0);
            Blitter.BlitCameraTexture(cmd, tempTexture, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // tempTexture is auto-managed by RTHandle; no need to ReleaseTemporaryRT
        }
    }

    RetroBlurPass blurPass;

    public override void Create()
    {
        blurPass = new RetroBlurPass(settings.blurMaterial)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!settings.blurMaterial)
            return;

        blurPass.SetBlur(settings.blurStrength);
        blurPass.Setup(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(blurPass);
    }
}
