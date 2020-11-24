//------------------------------------------------------------
//        File:  ImageExtensions.cs
//       Brief:  ImageExtensions
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:04
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

using UnityEngine;
using UnityEngine.UI;
using vFrame.Bundler.Base;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Extension.UI
{
    public static class ImageExtensions
    {
        private class SpriteSetter : PropertySetterProxy<Image, Sprite>
        {
            public override void Set(Image target, Sprite asset) {
                target.sprite = asset;
            }
        }

        private class OverrideSpriteSetter : PropertySetterProxy<Image, Sprite>
        {
            public override void Set(Image target, Sprite asset) {
                target.overrideSprite = asset;
            }
        }

        private class MaterialSetter : PropertySetterProxy<Image, Material>
        {
            public override void Set(Image target, Material asset) {
                target.material = asset;
            }
        }

        public static void SetSprite(this Image target, IAsset asset) {
            asset.SetTo<Image, Sprite, SpriteSetter>(target);
        }

        public static void SetOverrideSprite(this Image target, IAsset asset) {
            asset.SetTo<Image, Sprite, OverrideSpriteSetter>(target);
        }

        public static void SetMaterial(this Image target, IAsset asset) {
            asset.SetTo<Image, Material, MaterialSetter>(target);
        }
    }
}