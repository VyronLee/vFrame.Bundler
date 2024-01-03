// ------------------------------------------------------------
//         File: AssetAsync.cs
//        Brief: AssetAsync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 19:39
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    public class AssetAsync : Asset, IAssetAsync
    {
        public bool MoveNext() {
            return !IsDone;
        }

        public void Reset() {

        }

        public object Current => null;

        public bool IsDone => AssetLoader.IsDone;

        public float Progress => AssetLoader.Progress;
    }

    public class AssetAsync<T> : Asset<T>, IAssetAsync<T> where T : Object
    {
        public bool MoveNext() {
            return !IsDone;
        }

        public void Reset() {

        }

        public object Current => null;

        public bool IsDone => AssetLoader.IsDone;

        public float Progress => AssetLoader.Progress;
    }
}