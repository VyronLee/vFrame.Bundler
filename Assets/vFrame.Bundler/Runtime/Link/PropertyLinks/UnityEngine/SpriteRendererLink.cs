// ------------------------------------------------------------
//         File: SpriteRendererLink.cs
//        Brief: Routes loaded Sprite assets into SpriteRenderer.sprite with automatic reference tracking.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:06:44
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    public static class SpriteRendererLink
    {
        private class SpriteLink : PropertyLink<SpriteRenderer, Sprite>
        {
            public override void Set(SpriteRenderer target, Sprite asset)
            {
                target.sprite = asset;
            }
        }

        //============================================================
        // SetSprite
        //============================================================
        public static void SetSprite(this SpriteRenderer target, Asset asset)
        {
            asset.SetTo<SpriteRenderer, Sprite, SpriteLink>(target);
        }
        public static void SetSprite(this SpriteRenderer target, AssetAsync asset)
        {
            asset.SetTo<SpriteRenderer, Sprite, SpriteLink>(target);
        }
        public static void SetSprite(this SpriteRenderer target, Asset<Sprite> asset)
        {
            asset.SetTo<SpriteRenderer, SpriteLink>(target);
        }
        public static void SetSprite(this SpriteRenderer target, AssetAsync<Sprite> asset)
        {
            asset.SetTo<SpriteRenderer, SpriteLink>(target);
        }
    }
}