// ------------------------------------------------------------
//         File: SpriteRendererLink.cs
//        Brief: Property links that assign loaded Sprite assets to SpriteRenderer.sprite
//               with automatic reference tracking.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:10:35
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Extension methods that assign a loaded Sprite asset to a <see cref="SpriteRenderer"/>,
    /// using property links for automatic reference counting.
    /// </summary>
    public static class SpriteRendererLink
    {
        /// <summary>
        /// Property link that writes a <see cref="Sprite"/> into <see cref="SpriteRenderer.sprite"/>.
        /// </summary>
        private class SpriteLink : PropertyLink<SpriteRenderer, Sprite>
        {
            /// <summary>
            /// Assigns the sprite to the target renderer.
            /// </summary>
            /// <param name="target">Renderer receiving the sprite.</param>
            /// <param name="asset">Loaded sprite asset.</param>
            public override void Set(SpriteRenderer target, Sprite asset)
            {
                target.sprite = asset;
            }
        }

        //============================================================
        // SetSprite
        //============================================================
        /// <summary>
        /// Assigns the sprite of an untyped loaded asset to the renderer.
        /// </summary>
        /// <param name="target">Renderer receiving the sprite.</param>
        /// <param name="asset">Loaded asset wrapper holding a Sprite.</param>
        public static void SetSprite(this SpriteRenderer target, Asset asset)
        {
            asset.SetTo<SpriteRenderer, Sprite, SpriteLink>(target);
        }
        /// <summary>
        /// Assigns the sprite of an untyped asynchronously loaded asset to the renderer.
        /// </summary>
        /// <param name="target">Renderer receiving the sprite.</param>
        /// <param name="asset">Asynchronous asset wrapper holding a Sprite.</param>
        public static void SetSprite(this SpriteRenderer target, AssetAsync asset)
        {
            asset.SetTo<SpriteRenderer, Sprite, SpriteLink>(target);
        }
        /// <summary>
        /// Assigns the sprite of a typed loaded asset to the renderer.
        /// </summary>
        /// <param name="target">Renderer receiving the sprite.</param>
        /// <param name="asset">Loaded asset holding a Sprite.</param>
        public static void SetSprite(this SpriteRenderer target, Asset<Sprite> asset)
        {
            asset.SetTo<SpriteRenderer, SpriteLink>(target);
        }
        /// <summary>
        /// Assigns the sprite of a typed asynchronously loaded asset to the renderer.
        /// </summary>
        /// <param name="target">Renderer receiving the sprite.</param>
        /// <param name="asset">Asynchronous asset holding a Sprite.</param>
        public static void SetSprite(this SpriteRenderer target, AssetAsync<Sprite> asset)
        {
            asset.SetTo<SpriteRenderer, SpriteLink>(target);
        }
    }
}