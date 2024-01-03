using UnityEngine;
using vFrame.Bundler.Base;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Extension
{
    public static class ProjectorExtension
    {
        private class MaterialSetter : PropertySetterProxy<Projector, Material>
        {
            public override void Set(Projector target, Material asset) {
                target.material = asset;
            }
        }

        public static void SetMaterial(this Projector target, IAsset asset) {
            asset.SetTo<Projector, Material, MaterialSetter>(target);
        }
    }
}