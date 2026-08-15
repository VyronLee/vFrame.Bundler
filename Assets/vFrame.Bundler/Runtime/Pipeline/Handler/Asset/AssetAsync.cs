// ------------------------------------------------------------
//         File: AssetAsync.cs
//        Brief: Async asset handle structs (AssetAsync/AssetAsync<T>) implementing IAsync;
//               poll IsDone/Progress while the loader finishes, then access assets.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 19:39
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    public struct AssetAsync : ILoaderHandler, IAsync
    {
        private Loader _loader;
        Loader ILoaderHandler.Loader {
            get => _loader;
            set {
                // R6: load = +1 strong reference (matches Scene; Addressables/YooAsset
                // default). Without it the loader sits at References==0 and
                // CollectSystem reclaims it on the next Collect() — a dangling
                // handle. Unload() balances this retain.
                _loader = value;
                _loader.Retain();
            }
        }
        BundlerContexts ILoaderHandler.BundlerContexts { get; set; }

        void ILoaderHandler.Update()
        {

        }

        public UnloadOperation Unload()
        {
            if (IsUnloaded) {
                return UnloadOperation.Completed;
            }
            IsUnloaded = true;
            // Release the load-time retain. A loader already collected (destroyed)
            // no-ops via the R5 _destroyed guard, so this is safe across struct
            // copies that alias the same loader.
            _loader?.Release();
            return UnloadOperation.Completed;
        }

        [JsonSerializableProperty]
        public bool IsUnloaded { get; private set; }

        public Object GetRawAsset()
        {
            return AssetHelper<AssetAsync>.GetRawAsset(this);
        }

        public Object[] GetAllRawAssets()
        {
            return AssetHelper<AssetAsync>.GetAllRawAssets(this);
        }

        public Object Instantiate(Transform parent = null, bool stayWorldPosition = false)
        {
            return AssetHelper<AssetAsync>.Instantiate(this, parent, stayWorldPosition);
        }

        public void SetTo<TComponent, TLink, TProxy>(TComponent target)
            where TComponent : Component
            where TLink : Object
            where TProxy : PropertyLink<TComponent, TLink>, new()
        {

            AssetHelper<AssetAsync>.SetTo<TComponent, TLink, TProxy>(this, target);
        }

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {

        }

        public object Current => null;

        public bool IsDone => AssetHelper<AssetAsync>.GetAssetLoader(this).IsDone;

        public float Progress => AssetHelper<AssetAsync>.GetAssetLoader(this).Progress;
    }

    public struct AssetAsync<T> : ILoaderHandler, IAsync where T : Object
    {
        private Loader _loader;
        Loader ILoaderHandler.Loader {
            get => _loader;
            set {
                // R6: load = +1 strong reference (matches Scene; Addressables/YooAsset
                // default). Without it the loader sits at References==0 and
                // CollectSystem reclaims it on the next Collect() — a dangling
                // handle. Unload() balances this retain.
                _loader = value;
                _loader.Retain();
            }
        }
        BundlerContexts ILoaderHandler.BundlerContexts { get; set; }

        void ILoaderHandler.Update()
        {

        }

        public UnloadOperation Unload()
        {
            if (IsUnloaded) {
                return UnloadOperation.Completed;
            }
            IsUnloaded = true;
            // Release the load-time retain. A loader already collected (destroyed)
            // no-ops via the R5 _destroyed guard, so this is safe across struct
            // copies that alias the same loader.
            _loader?.Release();
            return UnloadOperation.Completed;
        }

        [JsonSerializableProperty]
        public bool IsUnloaded { get; private set; }

        public T GetRawAsset()
        {
            return AssetHelper<AssetAsync<T>>.GetRawAsset(this) as T;
        }

        public T[] GetAllRawAssets()
        {
            return AssetHelper<AssetAsync<T>>.GetAllRawAssets(this) as T[];
        }

        public T Instantiate(Transform parent = null, bool stayWorldPosition = false)
        {
            return AssetHelper<AssetAsync<T>>.Instantiate(this, parent, stayWorldPosition) as T;
        }

        public void SetTo<TComponent, TProxy>(TComponent target)
            where TComponent : Component
            where TProxy : PropertyLink<TComponent, T>, new()
        {

            AssetHelper<AssetAsync<T>>.SetTo<TComponent, T, TProxy>(this, target);
        }

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {

        }

        public object Current => null;

        public bool IsDone => AssetHelper<AssetAsync<T>>.GetAssetLoader(this).IsDone;

        public float Progress => AssetHelper<AssetAsync<T>>.GetAssetLoader(this).Progress;
    }
}