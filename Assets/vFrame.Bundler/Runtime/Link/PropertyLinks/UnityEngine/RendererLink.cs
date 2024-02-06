//------------------------------------------------------------
//        File:  RendererLink.cs
//       Brief:  RendererLink
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:04
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    public static class RendererLink
    {
        private class MaterialLink : PropertyLink<Renderer, Material>
        {
            public override void Set(Renderer target, Material asset) {
                target.material = asset;
            }
        }

        private class SharedMaterialLink : PropertyLink<Renderer, Material>
        {
            public override void Set(Renderer target, Material asset) {
                target.sharedMaterial = asset;
            }
        }

        //============================================================
        // SetMaterial
        //============================================================
        public static void SetMaterial(this Renderer target, Asset asset) {
            asset.SetTo<Renderer, Material, MaterialLink>(target);
        }
        public static void SetMaterial(this Renderer target, AssetAsync asset) {
            asset.SetTo<Renderer, Material, MaterialLink>(target);
        }
        public static void SetMaterial(this Renderer target, Asset<Material> asset) {
            asset.SetTo<Renderer, MaterialLink>(target);
        }
        public static void SetMaterial(this Renderer target, AssetAsync<Material> asset) {
            asset.SetTo<Renderer, MaterialLink>(target);
        }

        //============================================================
        // SetSharedMaterial
        //============================================================
        public static void SetSharedMaterial(this Renderer target, Asset asset) {
            asset.SetTo<Renderer, Material, SharedMaterialLink>(target);
        }
        public static void SetSharedMaterial(this Renderer target, AssetAsync asset) {
            asset.SetTo<Renderer, Material, SharedMaterialLink>(target);
        }
        public static void SetSharedMaterial(this Renderer target, Asset<Material> asset) {
            asset.SetTo<Renderer, SharedMaterialLink>(target);
        }
        public static void SetSharedMaterial(this Renderer target, AssetAsync<Material> asset) {
            asset.SetTo<Renderer, SharedMaterialLink>(target);
        }
    }
}