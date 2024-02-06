using UnityEngine;

namespace vFrame.Bundler
{
    public static class ProjectorLink
    {
        private class MaterialLink : PropertyLink<Projector, Material>
        {
            public override void Set(Projector target, Material asset) {
                target.material = asset;
            }
        }

        //============================================================
        // SetMaterial
        //============================================================
        public static void SetMaterial(this Projector target, Asset asset) {
            asset.SetTo<Projector, Material, MaterialLink>(target);
        }
        public static void SetMaterial(this Projector target, AssetAsync asset) {
            asset.SetTo<Projector, Material, MaterialLink>(target);
        }
        public static void SetMaterial(this Projector target, Asset<Material> asset) {
            asset.SetTo<Projector, MaterialLink>(target);
        }
        public static void SetMaterial(this Projector target, AssetAsync<Material> asset) {
            asset.SetTo<Projector, MaterialLink>(target);
        }
    }
}