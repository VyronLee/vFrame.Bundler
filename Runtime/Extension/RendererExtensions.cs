//------------------------------------------------------------
//        File:  RendererExtensions.cs
//       Brief:  RendererExtensions
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:04
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

using UnityEngine;
using vFrame.Bundler.Base;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Extension
{
    public static class RendererExtensions
    {
        private class MaterialSetter : PropertySetterProxy<Renderer, Material>
        {
            public override void Set(Renderer target, Material asset) {
                target.material = asset;
            }
        }

        private class SharedMaterialSetter : PropertySetterProxy<Renderer, Material>
        {
            public override void Set(Renderer target, Material asset) {
                target.sharedMaterial = asset;
            }
        }

        public static void SetMaterial(this Renderer target, IAsset asset) {
            asset.SetTo<Renderer, Material, MaterialSetter>(target);
        }

        public static void SetSharedMaterial(this Renderer target, IAsset asset) {
            asset.SetTo<Renderer, Material, SharedMaterialSetter>(target);
        }
    }
}