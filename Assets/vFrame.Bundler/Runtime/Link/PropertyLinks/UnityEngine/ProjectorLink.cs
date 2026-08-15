// ------------------------------------------------------------
//         File: ProjectorLink.cs
//        Brief: Routes loaded Material assets into Projector.material with automatic reference tracking.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:06:31
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    public static class ProjectorLink
    {
        private class MaterialLink : PropertyLink<Projector, Material>
        {
            public override void Set(Projector target, Material asset)
            {
                target.material = asset;
            }
        }

        //============================================================
        // SetMaterial
        //============================================================
        public static void SetMaterial(this Projector target, Asset asset)
        {
            asset.SetTo<Projector, Material, MaterialLink>(target);
        }
        public static void SetMaterial(this Projector target, AssetAsync asset)
        {
            asset.SetTo<Projector, Material, MaterialLink>(target);
        }
        public static void SetMaterial(this Projector target, Asset<Material> asset)
        {
            asset.SetTo<Projector, MaterialLink>(target);
        }
        public static void SetMaterial(this Projector target, AssetAsync<Material> asset)
        {
            asset.SetTo<Projector, MaterialLink>(target);
        }
    }
}