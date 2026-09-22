// ------------------------------------------------------------
//         File: ImageLink.cs
//        Brief: Property links that assign loaded Sprite and Material assets to Image with automatic
//               reference tracking.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:32:10
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;
using UnityEngine.UI;

namespace vFrame.Bundler
{
    /// <summary>
    /// Extension methods that assign loaded Sprite or Material assets to a <see cref="Image"/>,
    /// using property links for automatic reference counting.
    /// </summary>
    public static class ImageLink
    {
        /// <summary>
        /// Property link that writes a <see cref="Sprite"/> into <see cref="Image.sprite"/>.
        /// </summary>
        private class SpriteLink : PropertyLink<Image, Sprite>
        {
            /// <summary>
            /// Assigns the sprite to the target image.
            /// </summary>
            /// <param name="target">Image receiving the sprite.</param>
            /// <param name="asset">Loaded sprite asset.</param>
            public override void Set(Image target, Sprite asset)
            {
                target.sprite = asset;
            }
        }

        /// <summary>
        /// Property link that writes a <see cref="Sprite"/> into <see cref="Image.overrideSprite"/>.
        /// </summary>
        private class OverrideSpriteLink : PropertyLink<Image, Sprite>
        {
            /// <summary>
            /// Assigns the override sprite to the target image.
            /// </summary>
            /// <param name="target">Image receiving the override sprite.</param>
            /// <param name="asset">Loaded sprite asset.</param>
            public override void Set(Image target, Sprite asset)
            {
                target.overrideSprite = asset;
            }
        }

        /// <summary>
        /// Property link that writes a <see cref="Material"/> into <see cref="Image.material"/>.
        /// </summary>
        private class MaterialLink : PropertyLink<Image, Material>
        {
            /// <summary>
            /// Assigns the material to the target image.
            /// </summary>
            /// <param name="target">Image receiving the material.</param>
            /// <param name="asset">Loaded material asset.</param>
            public override void Set(Image target, Material asset)
            {
                target.material = asset;
            }
        }

        //============================================================
        // SetSprite
        //============================================================
        /// <summary>
        /// Assigns the sprite of an untyped loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the sprite.</param>
        /// <param name="asset">Loaded asset wrapper holding a Sprite.</param>
        public static void SetSprite(this Image target, Asset asset)
        {
            asset.SetTo<Image, Sprite, SpriteLink>(target);
        }
        /// <summary>
        /// Assigns the sprite of an untyped asynchronously loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the sprite.</param>
        /// <param name="asset">Asynchronous asset wrapper holding a Sprite.</param>
        public static void SetSprite(this Image target, AssetAsync asset)
        {
            asset.SetTo<Image, Sprite, SpriteLink>(target);
        }
        /// <summary>
        /// Assigns the sprite of a typed loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the sprite.</param>
        /// <param name="asset">Loaded asset holding a Sprite.</param>
        public static void SetSprite(this Image target, Asset<Sprite> asset)
        {
            asset.SetTo<Image, SpriteLink>(target);
        }
        /// <summary>
        /// Assigns the sprite of a typed asynchronously loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the sprite.</param>
        /// <param name="asset">Asynchronous asset holding a Sprite.</param>
        public static void SetSprite(this Image target, AssetAsync<Sprite> asset)
        {
            asset.SetTo<Image, SpriteLink>(target);
        }

        //============================================================
        // SetOverrideSprite
        //============================================================
        /// <summary>
        /// Assigns the override sprite of an untyped loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the override sprite.</param>
        /// <param name="asset">Loaded asset wrapper holding a Sprite.</param>
        public static void SetOverrideSprite(this Image target, Asset asset)
        {
            asset.SetTo<Image, Sprite, OverrideSpriteLink>(target);
        }
        /// <summary>
        /// Assigns the override sprite of an untyped asynchronously loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the override sprite.</param>
        /// <param name="asset">Asynchronous asset wrapper holding a Sprite.</param>
        public static void SetOverrideSprite(this Image target, AssetAsync asset)
        {
            asset.SetTo<Image, Sprite, OverrideSpriteLink>(target);
        }
        /// <summary>
        /// Assigns the override sprite of a typed loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the override sprite.</param>
        /// <param name="asset">Loaded asset holding a Sprite.</param>
        public static void SetOverrideSprite(this Image target, Asset<Sprite> asset)
        {
            asset.SetTo<Image, OverrideSpriteLink>(target);
        }
        /// <summary>
        /// Assigns the override sprite of a typed asynchronously loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the override sprite.</param>
        /// <param name="asset">Asynchronous asset holding a Sprite.</param>
        public static void SetOverrideSprite(this Image target, AssetAsync<Sprite> asset)
        {
            asset.SetTo<Image, OverrideSpriteLink>(target);
        }

        //============================================================
        // SetMaterial
        //============================================================
        /// <summary>
        /// Assigns the material of an untyped loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the material.</param>
        /// <param name="asset">Loaded asset wrapper holding a Material.</param>
        public static void SetMaterial(this Image target, Asset asset)
        {
            asset.SetTo<Image, Material, MaterialLink>(target);
        }
        /// <summary>
        /// Assigns the material of an untyped asynchronously loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the material.</param>
        /// <param name="asset">Asynchronous asset wrapper holding a Material.</param>
        public static void SetMaterial(this Image target, AssetAsync asset)
        {
            asset.SetTo<Image, Material, MaterialLink>(target);
        }
        /// <summary>
        /// Assigns the material of a typed loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the material.</param>
        /// <param name="asset">Loaded asset holding a Material.</param>
        public static void SetMaterial(this Image target, Asset<Material> asset)
        {
            asset.SetTo<Image, MaterialLink>(target);
        }
        /// <summary>
        /// Assigns the material of a typed asynchronously loaded asset to the image.
        /// </summary>
        /// <param name="target">Image receiving the material.</param>
        /// <param name="asset">Asynchronous asset holding a Material.</param>
        public static void SetMaterial(this Image target, AssetAsync<Material> asset)
        {
            asset.SetTo<Image, MaterialLink>(target);
        }
    }
}