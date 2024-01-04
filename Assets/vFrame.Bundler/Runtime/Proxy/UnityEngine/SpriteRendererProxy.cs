//------------------------------------------------------------
//        File:  SpriteRendererProxy.cs
//       Brief:  SpriteRendererProxy
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2020-05-21 16:20
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    public static class SpriteRendererProxy
    {
        private class SpriteSetter : PropertySetterProxy<SpriteRenderer, Sprite>
        {
            public override void Set(SpriteRenderer target, Sprite asset) {
                target.sprite = asset;
            }
        }

        //============================================================
        // SetSprite
        //============================================================
        public static void SetSprite(this SpriteRenderer target, Asset asset) {
            asset.SetTo<SpriteRenderer, Sprite, SpriteSetter>(target);
        }
        public static void SetSprite(this SpriteRenderer target, AssetAsync asset) {
            asset.SetTo<SpriteRenderer, Sprite, SpriteSetter>(target);
        }
        public static void SetSprite(this SpriteRenderer target, Asset<Sprite> asset) {
            asset.SetTo<SpriteRenderer, SpriteSetter>(target);
        }
        public static void SetSprite(this SpriteRenderer target, AssetAsync<Sprite> asset) {
            asset.SetTo<SpriteRenderer, SpriteSetter>(target);
        }
    }
}