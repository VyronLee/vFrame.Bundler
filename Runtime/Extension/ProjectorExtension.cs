using UnityEngine;

namespace vFrame.Bundler
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