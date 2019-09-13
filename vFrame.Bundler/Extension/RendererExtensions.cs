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
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Extension
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