//------------------------------------------------------------
//        File:  ImageProxy.cs
//       Brief:  ImageProxy
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:04
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using UnityEngine;
using UnityEngine.UI;

namespace vFrame.Bundler
{
    public static class ImageProxy
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

        //============================================================
        // SetSprite
        //============================================================
        public static void SetSprite(this Image target, Asset asset) {
            asset.SetTo<Image, Sprite, SpriteSetter>(target);
        }
        public static void SetSprite(this Image target, AssetAsync asset) {
            asset.SetTo<Image, Sprite, SpriteSetter>(target);
        }
        public static void SetSprite(this Image target, Asset<Sprite> asset) {
            asset.SetTo<Image, SpriteSetter>(target);
        }
        public static void SetSprite(this Image target, AssetAsync<Sprite> asset) {
            asset.SetTo<Image, SpriteSetter>(target);
        }

        //============================================================
        // SetOverrideSprite
        //============================================================
        public static void SetOverrideSprite(this Image target, Asset asset) {
            asset.SetTo<Image, Sprite, OverrideSpriteSetter>(target);
        }
        public static void SetOverrideSprite(this Image target, AssetAsync asset) {
            asset.SetTo<Image, Sprite, OverrideSpriteSetter>(target);
        }
        public static void SetOverrideSprite(this Image target, Asset<Sprite> asset) {
            asset.SetTo<Image, OverrideSpriteSetter>(target);
        }
        public static void SetOverrideSprite(this Image target, AssetAsync<Sprite> asset) {
            asset.SetTo<Image, OverrideSpriteSetter>(target);
        }

        //============================================================
        // SetMaterial
        //============================================================
        public static void SetMaterial(this Image target, Asset asset) {
            asset.SetTo<Image, Material, MaterialSetter>(target);
        }
        public static void SetMaterial(this Image target, AssetAsync asset) {
            asset.SetTo<Image, Material, MaterialSetter>(target);
        }
        public static void SetMaterial(this Image target, Asset<Material> asset) {
            asset.SetTo<Image, MaterialSetter>(target);
        }
        public static void SetMaterial(this Image target, AssetAsync<Material> asset) {
            asset.SetTo<Image, MaterialSetter>(target);
        }
    }
}