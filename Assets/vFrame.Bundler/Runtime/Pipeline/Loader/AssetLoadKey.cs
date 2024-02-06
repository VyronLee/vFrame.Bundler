// ------------------------------------------------------------
//         File: AssetLoadKey.cs
//        Brief: AssetLoadKey.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 19:7
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;

namespace vFrame.Bundler
{
    internal class AssetLoadKey : Tuple<string, Type>, IEquatable<AssetLoadKey>
    {
        private AssetLoadKey(string item1, Type item2) : base(item1, item2) {

        }

        public static AssetLoadKey Create(string item1, Type item2) {
            return new AssetLoadKey(item1, item2);
        }

        public bool Equals(AssetLoadKey other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            return Item1 == other.Item1 && Item2 == other.Item2;
        }

        public override bool Equals(object obj) {
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

        public override int GetHashCode() {
            return Item1.GetHashCode() & Item2.GetHashCode();
        }

        public static implicit operator AssetLoadKey((string, Type) other) {
            return Create(other.Item1, other.Item2);
        }
    }
}