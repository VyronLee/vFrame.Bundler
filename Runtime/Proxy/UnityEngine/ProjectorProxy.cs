using UnityEngine;

namespace vFrame.Bundler
{
    public static class ProjectorProxy
    {
        private class MaterialSetter : PropertySetterProxy<Projector, Material>
        {
            public override void Set(Projector target, Material asset) {
                target.material = asset;
            }
        }

        //============================================================
        // SetMaterial
        //============================================================
        public static void SetMaterial(this Projector target, Asset asset) {
            asset.SetTo<Projector, Material, MaterialSetter>(target);
        }
        public static void SetMaterial(this Projector target, AssetAsync asset) {
            asset.SetTo<Projector, Material, MaterialSetter>(target);
        }
        public static void SetMaterial(this Projector target, Asset<Material> asset) {
            asset.SetTo<Projector, MaterialSetter>(target);
        }
        public static void SetMaterial(this Projector target, AssetAsync<Material> asset) {
            asset.SetTo<Projector, MaterialSetter>(target);
        }
    }
}