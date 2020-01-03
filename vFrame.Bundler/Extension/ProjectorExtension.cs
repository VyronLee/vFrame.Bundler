using UnityEngine;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Extension
{
    public static class ProjectorExtension
    {
        public static void SetMaterial(this Projector target, IAsset asset)
        {
            asset.SetTo(target, "material");
        }
    }
}