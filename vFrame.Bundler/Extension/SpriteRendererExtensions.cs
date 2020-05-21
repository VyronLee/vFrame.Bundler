//------------------------------------------------------------
//        File:  SpriteRendererExtensions.cs
//       Brief:  SpriteRendererExtensions
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2020-05-21 16:20
//   Copyright:  Copyright (c) 2020, VyronLee
//============================================================
using UnityEngine;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Extension
{
    public static class SpriteRendererExtensions
    {
        public static void SetSprite(this SpriteRenderer target, IAsset asset)
        {
            asset.SetTo(target, "sprite");
        }
    }
}