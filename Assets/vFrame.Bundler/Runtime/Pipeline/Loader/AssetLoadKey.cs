// ------------------------------------------------------------
//         File: AssetLoadKey.cs
//        Brief: Composite (assetPath, assetType) tuple key used to index asset loaders; value equality + hash.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:44:27
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Composite (assetPath, assetType) key used to index asset loaders, with value-based equality and hashing.
    /// </summary>
    internal class AssetLoadKey : Tuple<string, Type>, IEquatable<AssetLoadKey>
    {
        /// <summary>
        ///     Initializes the key with the specified asset path and asset type.
        /// </summary>
        /// <param name="item1">Asset path of the asset.</param>
        /// <param name="item2">System type of the asset.</param>
        private AssetLoadKey(string item1, Type item2) : base(item1, item2)
        {

        }

        /// <summary>
        ///     Creates a new asset load key from the specified asset path and asset type.
        /// </summary>
        /// <param name="item1">Asset path of the asset.</param>
        /// <param name="item2">System type of the asset.</param>
        /// <returns>A new <see cref="AssetLoadKey"/> combining both values.</returns>
        public static AssetLoadKey Create(string item1, Type item2)
        {
            return new AssetLoadKey(item1, item2);
        }

        /// <summary>
        ///     Indicates whether the current key equals another key by comparing path and type values.
        /// </summary>
        /// <param name="other">The key to compare with this instance, or null.</param>
        /// <returns>true if both path and type match; otherwise, false.</returns>
        public bool Equals(AssetLoadKey other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            return Item1 == other.Item1 && Item2 == other.Item2;
        }

        /// <summary>
        ///     Determines whether the specified object is an <see cref="AssetLoadKey"/> with the same path and type values.
        /// </summary>
        /// <param name="obj">The object to compare with this instance, or null.</param>
        /// <returns>true if the object is an equivalent <see cref="AssetLoadKey"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((AssetLoadKey)obj);
        }

        /// <summary>
        ///     Returns a hash code combining the hash codes of the path and type values.
        /// </summary>
        /// <returns>A hash code for the current key.</returns>
        public override int GetHashCode()
        {
            return Item1.GetHashCode() & Item2.GetHashCode();
        }

        /// <summary>
        ///     Implicitly converts a (assetPath, assetType) value tuple into an <see cref="AssetLoadKey"/>.
        /// </summary>
        /// <param name="other">The tuple containing the asset path and asset type.</param>
        /// <returns>A new <see cref="AssetLoadKey"/> created from the tuple values.</returns>
        public static implicit operator AssetLoadKey((string, Type) other)
        {
            return Create(other.Item1, other.Item2);
        }
    }
}