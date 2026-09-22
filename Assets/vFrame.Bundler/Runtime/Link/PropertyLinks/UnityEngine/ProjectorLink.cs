// ------------------------------------------------------------
//         File: ProjectorLink.cs
//        Brief: Assigns loaded Material assets to Projector.material with automatic reference tracking.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:10:26
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Property-link extensions that assign a bundler-loaded <see cref="Material"/> asset to a
    /// <see cref="Projector"/>'s material slot, keeping the asset reference-counted until relinked.
    /// </summary>
    public static class ProjectorLink
    {
        /// <summary>
        /// <see cref="PropertyLink{TComponent,TObject}"/> that writes the linked asset into
        /// <see cref="Projector.material"/>, replacing the previous material instance.
        /// </summary>
        private class MaterialLink : PropertyLink<Projector, Material>
        {
            /// <inheritdoc />
            public override void Set(Projector target, Material asset)
            {
                target.material = asset;
            }
        }

        //============================================================
        // SetMaterial
        //============================================================

        /// <summary>
        /// Links a type-erased loaded asset to the projector's material. The asset object must be
        /// a <see cref="Material"/>; mismatches are logged and ignored.
        /// </summary>
        /// <param name="target">Projector whose material receives the asset.</param>
        /// <param name="asset">Type-erased asset handle holding the material.</param>
        public static void SetMaterial(this Projector target, Asset asset)
        {
            asset.SetTo<Projector, Material, MaterialLink>(target);
        }

        /// <summary>
        /// Links a type-erased asynchronously loading asset to the projector's material once the
        /// load completes. The asset object must be a <see cref="Material"/>; mismatches are
        /// logged and ignored.
        /// </summary>
        /// <param name="target">Projector whose material receives the asset.</param>
        /// <param name="asset">Type-erased asynchronous asset handle holding the material.</param>
        public static void SetMaterial(this Projector target, AssetAsync asset)
        {
            asset.SetTo<Projector, Material, MaterialLink>(target);
        }

        /// <summary>
        /// Links a typed loaded material asset to the projector's material, releasing the
        /// projector's previous material link.
        /// </summary>
        /// <param name="target">Projector whose material receives the asset.</param>
        /// <param name="asset">Loaded asset handle holding a <see cref="Material"/>.</param>
        public static void SetMaterial(this Projector target, Asset<Material> asset)
        {
            asset.SetTo<Projector, MaterialLink>(target);
        }

        /// <summary>
        /// Links a typed asynchronously loading material asset to the projector's material once
        /// the load completes, releasing the projector's previous material link.
        /// </summary>
        /// <param name="target">Projector whose material receives the asset.</param>
        /// <param name="asset">Asynchronous asset handle holding a <see cref="Material"/>.</param>
        public static void SetMaterial(this Projector target, AssetAsync<Material> asset)
        {
            asset.SetTo<Projector, MaterialLink>(target);
        }
    }
}