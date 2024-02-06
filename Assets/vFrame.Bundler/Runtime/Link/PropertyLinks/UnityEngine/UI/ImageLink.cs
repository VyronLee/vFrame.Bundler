//------------------------------------------------------------
//        File:  ImageLink.cs
//       Brief:  ImageLink
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
    public static class ImageLink
    {
        private class SpriteLink : PropertyLink<Image, Sprite>
        {
            public override void Set(Image target, Sprite asset) {
                target.sprite = asset;
            }
        }

        private class OverrideSpriteLink : PropertyLink<Image, Sprite>
        {
            public override void Set(Image target, Sprite asset) {
                target.overrideSprite = asset;
            }
        }

        private class MaterialLink : PropertyLink<Image, Material>
        {
            public override void Set(Image target, Material asset) {
                target.material = asset;
            }
        }

        //============================================================
        // SetSprite
        //============================================================
        public static void SetSprite(this Image target, Asset asset) {
            asset.SetTo<Image, Sprite, SpriteLink>(target);
        }
        public static void SetSprite(this Image target, AssetAsync asset) {
            asset.SetTo<Image, Sprite, SpriteLink>(target);
        }
        public static void SetSprite(this Image target, Asset<Sprite> asset) {
            asset.SetTo<Image, SpriteLink>(target);
        }
        public static void SetSprite(this Image target, AssetAsync<Sprite> asset) {
            asset.SetTo<Image, SpriteLink>(target);
        }

        //============================================================
        // SetOverrideSprite
        //============================================================
        public static void SetOverrideSprite(this Image target, Asset asset) {
            asset.SetTo<Image, Sprite, OverrideSpriteLink>(target);
        }
        public static void SetOverrideSprite(this Image target, AssetAsync asset) {
            asset.SetTo<Image, Sprite, OverrideSpriteLink>(target);
        }
        public static void SetOverrideSprite(this Image target, Asset<Sprite> asset) {
            asset.SetTo<Image, OverrideSpriteLink>(target);
        }
        public static void SetOverrideSprite(this Image target, AssetAsync<Sprite> asset) {
            asset.SetTo<Image, OverrideSpriteLink>(target);
        }

        //============================================================
        // SetMaterial
        //============================================================
        public static void SetMaterial(this Image target, Asset asset) {
            asset.SetTo<Image, Material, MaterialLink>(target);
        }
        public static void SetMaterial(this Image target, AssetAsync asset) {
            asset.SetTo<Image, Material, MaterialLink>(target);
        }
        public static void SetMaterial(this Image target, Asset<Material> asset) {
            asset.SetTo<Image, MaterialLink>(target);
        }
        public static void SetMaterial(this Image target, AssetAsync<Material> asset) {
            asset.SetTo<Image, MaterialLink>(target);
        }
    }
}