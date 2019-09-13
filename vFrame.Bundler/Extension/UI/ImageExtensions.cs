//------------------------------------------------------------
//        File:  ImageExtensions.cs
//       Brief:  ImageExtensions
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:04
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

using UnityEngine.UI;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Extension.UI
{
    public static class ImageExtensions
    {
        public static void SetSprite(this Image target, IAsset asset)
        {
            asset.SetTo(target, "sprite");
        }

        public static void SetOverrideSprite(this Image target, IAsset asset)
        {
            asset.SetTo(target, "overrideSprite");
        }

        public static void SetMaterial(this Image target, IAsset asset)
        {
            asset.SetTo(target, "material");
        }
    }
}