using UnityEngine;
using vBundler.Interface;

namespace vBundler.Extension
{
    public static class RendererExtensions
    {
        public static void SetMaterial(this Renderer target, IAsset asset)
        {
            asset.SetTo(target, "material");
        }

        public static void SetSharedMaterial(this Renderer target, IAsset asset)
        {
            asset.SetTo(target, "sharedMaterial");
        }
    }
}