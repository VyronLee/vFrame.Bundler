// ------------------------------------------------------------
//         File: RendererLink.cs
//        Brief: Routes loaded Material assets into Renderer.material/sharedMaterial with automatic reference tracking.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:10:30
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Extension methods that route a loaded Material asset into a <see cref="Renderer"/>'s material
    ///     property, with automatic reference tracking through the link system.
    /// </summary>
    public static class RendererLink
    {
        /// <summary>
        ///     Links a Material asset to <see cref="Renderer.material"/> (instantiated per-renderer material).
        /// </summary>
        private class MaterialLink : PropertyLink<Renderer, Material>
        {
            /// <inheritdoc cref="PropertyLink{T1,T2}.Set"/>
            public override void Set(Renderer target, Material asset)
            {
                target.material = asset;
            }
        }
        /// <summary>
        ///     Links a Material asset to <see cref="Renderer.sharedMaterial"/> (the shared asset, not instanced).
        /// </summary>
        private class SharedMaterialLink : PropertyLink<Renderer, Material>
        {
            /// <inheritdoc cref="PropertyLink{T1,T2}.Set"/>
            public override void Set(Renderer target, Material asset)
            {
                target.sharedMaterial = asset;
            }
        }
        /// <summary>
        ///     Assigns the material of a loaded asset to <paramref name="target"/>.material,
        ///     replacing any previous link and tracking the reference until relinked or destroyed.
        /// </summary>
        /// <param name="target">Renderer to receive the material.</param>
        /// <param name="asset">Loaded asset handle whose object must be a Material.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="asset"/> is not an asset handle.</exception>
        public static void SetMaterial(this Renderer target, Asset asset)
        {
            asset.SetTo<Renderer, Material, MaterialLink>(target);
        }
        /// <summary>
        ///     Asynchronously assigns the material of a loading asset to <paramref name="target"/>.material
        ///     once the asset completes, replacing any previous link.
        /// </summary>
        /// <param name="target">Renderer to receive the material.</param>
        /// <param name="asset">Asynchronous asset handle whose object must be a Material.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="asset"/> is not an asset handle.</exception>
        public static void SetMaterial(this Renderer target, AssetAsync asset)
        {
            asset.SetTo<Renderer, Material, MaterialLink>(target);
        }
        /// <summary>
        ///     Assigns a loaded <see cref="Material"/> asset to <paramref name="target"/>.material,
        ///     replacing any previous link and tracking the reference until relinked or destroyed.
        /// </summary>
        /// <param name="target">Renderer to receive the material.</param>
        /// <param name="asset">Loaded Material asset handle.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="asset"/> is not an asset handle.</exception>
        public static void SetMaterial(this Renderer target, Asset<Material> asset)
        {
            asset.SetTo<Renderer, MaterialLink>(target);
        }
        /// <summary>
        ///     Asynchronously assigns a loading <see cref="Material"/> asset to <paramref name="target"/>.material
        ///     once the asset completes, replacing any previous link.
        /// </summary>
        /// <param name="target">Renderer to receive the material.</param>
        /// <param name="asset">Asynchronous Material asset handle.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="asset"/> is not an asset handle.</exception>
        public static void SetMaterial(this Renderer target, AssetAsync<Material> asset)
        {
            asset.SetTo<Renderer, MaterialLink>(target);
        }
        /// <summary>
        ///     Assigns the material of a loaded asset to <paramref name="target"/>.sharedMaterial
        ///     (the shared asset itself, not an instanced copy), replacing any previous link.
        /// </summary>
        /// <param name="target">Renderer to receive the material.</param>
        /// <param name="asset">Loaded asset handle whose object must be a Material.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="asset"/> is not an asset handle.</exception>
        public static void SetSharedMaterial(this Renderer target, Asset asset)
        {
            asset.SetTo<Renderer, Material, SharedMaterialLink>(target);
        }
        /// <summary>
        ///     Asynchronously assigns the material of a loading asset to <paramref name="target"/>.sharedMaterial
        ///     (the shared asset itself, not an instanced copy) once the asset completes, replacing any previous link.
        /// </summary>
        /// <param name="target">Renderer to receive the material.</param>
        /// <param name="asset">Asynchronous asset handle whose object must be a Material.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="asset"/> is not an asset handle.</exception>
        public static void SetSharedMaterial(this Renderer target, AssetAsync asset)
        {
            asset.SetTo<Renderer, Material, SharedMaterialLink>(target);
        }
        /// <summary>
        ///     Assigns a loaded <see cref="Material"/> asset to <paramref name="target"/>.sharedMaterial
        ///     (the shared asset itself, not an instanced copy), replacing any previous link.
        /// </summary>
        /// <param name="target">Renderer to receive the material.</param>
        /// <param name="asset">Loaded Material asset handle.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="asset"/> is not an asset handle.</exception>
        public static void SetSharedMaterial(this Renderer target, Asset<Material> asset)
        {
            asset.SetTo<Renderer, SharedMaterialLink>(target);
        }
        /// <summary>
        ///     Asynchronously assigns a loading <see cref="Material"/> asset to <paramref name="target"/>.sharedMaterial
        ///     (the shared asset itself, not an instanced copy) once the asset completes, replacing any previous link.
        /// </summary>
        /// <param name="target">Renderer to receive the material.</param>
        /// <param name="asset">Asynchronous Material asset handle.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="asset"/> is not an asset handle.</exception>
        public static void SetSharedMaterial(this Renderer target, AssetAsync<Material> asset)
        {
            asset.SetTo<Renderer, SharedMaterialLink>(target);
        }
    }
}