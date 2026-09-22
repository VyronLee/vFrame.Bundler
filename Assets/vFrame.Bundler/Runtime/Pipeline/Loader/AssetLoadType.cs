// ------------------------------------------------------------
//         File: AssetLoadType.cs
//        Brief: Defines how an asset request loads content: main asset only, with sub-assets, or all assets.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:44:31
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Specifies how an asset request loads content from its container.
    /// </summary>
    public enum AssetLoadType
    {
        /// <summary>
        /// Load only the main asset of the target path.
        /// </summary>
        LoadAsset,

        /// <summary>
        /// Load the main asset together with all of its sub-assets.
        /// </summary>
        LoadAssetWithSubAsset,

        /// <summary>
        /// Load every asset contained in the target.
        /// </summary>
        LoadAllAssets,
    }
}