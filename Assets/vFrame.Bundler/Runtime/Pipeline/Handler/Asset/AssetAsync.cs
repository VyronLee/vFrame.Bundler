// ------------------------------------------------------------
//         File: AssetAsync.cs
//        Brief: Async asset handle structs (AssetAsync/AssetAsync<T>) wrapping an in-flight
//               load: poll/yield until IsDone, then access assets; Unload releases the ref.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:20:19
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Asynchronous handle for a single asset load. Poll <see cref="IsDone"/>/<see cref="Progress"/>
    ///     or yield the handle in a coroutine until loading completes, then access the loaded asset;
    ///     call <see cref="Unload"/> to release the underlying loader reference.
    /// </summary>
    public struct AssetAsync : ILoaderHandler, IAsync
    {
        /// <summary>Underlying loader; retained on assignment, released on <see cref="Unload"/>.</summary>
        private Loader _loader;
        /// <summary>Retains the assigned loader so it survives until <see cref="Unload"/> balances the reference.</summary>
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

        /// <summary>Per-frame tick; performs no work.</summary>
        void ILoaderHandler.Update()
        {

        }

        /// <summary>
        ///     Marks the handle as unloaded and releases the load-time reference on the underlying loader.
        /// </summary>
        /// <returns>Always <see cref="UnloadOperation.Completed"/>.</returns>
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

        /// <summary>Gets a value indicating whether <see cref="Unload"/> has been called on this handle.</summary>
        [JsonSerializableProperty]
        public bool IsUnloaded { get; private set; }

        /// <summary>
        ///     Gets the main loaded asset object; meaningful only once <see cref="IsDone"/> is true.
        /// </summary>
        /// <exception cref="ArgumentException">The handle is not bound to an asset loader.</exception>
        public Object GetRawAsset()
        {
            return AssetHelper<AssetAsync>.GetRawAsset(this);
        }

        /// <summary>
        ///     Gets all loaded asset objects; meaningful only once <see cref="IsDone"/> is true.
        /// </summary>
        /// <exception cref="ArgumentException">The handle is not bound to an asset loader.</exception>
        public Object[] GetAllRawAssets()
        {
            return AssetHelper<AssetAsync>.GetAllRawAssets(this);
        }

        /// <summary>
        ///     Instantiates the loaded asset and links its properties for automatic reference tracking.
        /// </summary>
        /// <param name="parent">Parent transform for the instance, or <c>null</c>.</param>
        /// <param name="stayWorldPosition">Keep the instance's world position when parented.</param>
        /// <returns>The instantiated instance.</returns>
        public Object Instantiate(Transform parent = null, bool stayWorldPosition = false)
        {
            return AssetHelper<AssetAsync>.Instantiate(this, parent, stayWorldPosition);
        }

        /// <summary>
        ///     Binds the properties of a component to the loaded asset through the specified link proxy.
        /// </summary>
        /// <typeparam name="TComponent">Component type owning the linked properties.</typeparam>
        /// <typeparam name="TLink">Linked asset object type.</typeparam>
        /// <typeparam name="TProxy">Property link implementation describing the bindings.</typeparam>
        /// <param name="target">Component whose properties are bound.</param>
        public void SetTo<TComponent, TLink, TProxy>(TComponent target)
            where TComponent : Component
            where TLink : Object
            where TProxy : PropertyLink<TComponent, TLink>, new()
        {

            AssetHelper<AssetAsync>.SetTo<TComponent, TLink, TProxy>(this, target);
        }

        /// <summary>
        ///     IEnumerator support; advances while loading is incomplete so the handle can be yielded in a coroutine.
        /// </summary>
        /// <returns><c>true</c> while the load is still running.</returns>
        public bool MoveNext()
        {
            return !IsDone;
        }

        /// <summary>IEnumerator support; no-op.</summary>
        public void Reset()
        {

        }

        /// <summary>IEnumerator support; always <c>null</c>.</summary>
        public object Current => null;

        /// <summary>Gets a value indicating whether the load has completed.</summary>
        /// <exception cref="ArgumentException">The handle is not bound to an asset loader.</exception>
        public bool IsDone => AssetHelper<AssetAsync>.GetAssetLoader(this).IsDone;

        /// <summary>Gets the normalized load progress, ranging from 0 to 1.</summary>
        /// <exception cref="ArgumentException">The handle is not bound to an asset loader.</exception>
        public float Progress => AssetHelper<AssetAsync>.GetAssetLoader(this).Progress;
    }

    /// <summary>
    ///     Typed asynchronous handle for a single asset load of type <typeparamref name="T"/>.
    ///     Poll <see cref="IsDone"/>/<see cref="Progress"/> or yield the handle in a coroutine until loading
    ///     completes, then access the loaded asset; call <see cref="Unload"/> to release the loader reference.
    /// </summary>
    /// <typeparam name="T">Asset type returned by the accessors.</typeparam>
    public struct AssetAsync<T> : ILoaderHandler, IAsync where T : Object
    {
        /// <summary>Underlying loader; retained on assignment, released on <see cref="Unload"/>.</summary>
        private Loader _loader;
        /// <summary>Retains the assigned loader so it survives until <see cref="Unload"/> balances the reference.</summary>
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

        /// <summary>Per-frame tick; performs no work.</summary>
        void ILoaderHandler.Update()
        {

        }

        /// <summary>
        ///     Marks the handle as unloaded and releases the load-time reference on the underlying loader.
        /// </summary>
        /// <returns>Always <see cref="UnloadOperation.Completed"/>.</returns>
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

        /// <summary>Gets a value indicating whether <see cref="Unload"/> has been called on this handle.</summary>
        [JsonSerializableProperty]
        public bool IsUnloaded { get; private set; }

        /// <summary>
        ///     Gets the main loaded asset cast to <typeparamref name="T"/>; meaningful only once
        ///     <see cref="IsDone"/> is true.
        /// </summary>
        /// <exception cref="ArgumentException">The handle is not bound to an asset loader.</exception>
        public T GetRawAsset()
        {
            return AssetHelper<AssetAsync<T>>.GetRawAsset(this) as T;
        }

        /// <summary>
        ///     Gets all loaded assets cast to <typeparamref name="T"/>; meaningful only once
        ///     <see cref="IsDone"/> is true.
        /// </summary>
        /// <exception cref="ArgumentException">The handle is not bound to an asset loader.</exception>
        public T[] GetAllRawAssets()
        {
            return AssetHelper<AssetAsync<T>>.GetAllRawAssets(this) as T[];
        }

        /// <summary>
        ///     Instantiates the loaded asset and links its properties for automatic reference tracking.
        /// </summary>
        /// <param name="parent">Parent transform for the instance, or <c>null</c>.</param>
        /// <param name="stayWorldPosition">Keep the instance's world position when parented.</param>
        /// <returns>The instantiated instance cast to <typeparamref name="T"/>.</returns>
        public T Instantiate(Transform parent = null, bool stayWorldPosition = false)
        {
            return AssetHelper<AssetAsync<T>>.Instantiate(this, parent, stayWorldPosition) as T;
        }

        /// <summary>
        ///     Binds the properties of a component to the loaded asset through the specified link proxy.
        /// </summary>
        /// <typeparam name="TComponent">Component type owning the linked properties.</typeparam>
        /// <typeparam name="TProxy">Property link implementation describing the bindings.</typeparam>
        /// <param name="target">Component whose properties are bound.</param>
        public void SetTo<TComponent, TProxy>(TComponent target)
            where TComponent : Component
            where TProxy : PropertyLink<TComponent, T>, new()
        {

            AssetHelper<AssetAsync<T>>.SetTo<TComponent, T, TProxy>(this, target);
        }

        /// <summary>
        ///     IEnumerator support; advances while loading is incomplete so the handle can be yielded in a coroutine.
        /// </summary>
        /// <returns><c>true</c> while the load is still running.</returns>
        public bool MoveNext()
        {
            return !IsDone;
        }

        /// <summary>IEnumerator support; no-op.</summary>
        public void Reset()
        {

        }

        /// <summary>IEnumerator support; always <c>null</c>.</summary>
        public object Current => null;

        /// <summary>Gets a value indicating whether the load has completed.</summary>
        /// <exception cref="ArgumentException">The handle is not bound to an asset loader.</exception>
        public bool IsDone => AssetHelper<AssetAsync<T>>.GetAssetLoader(this).IsDone;

        /// <summary>Gets the normalized load progress, ranging from 0 to 1.</summary>
        /// <exception cref="ArgumentException">The handle is not bound to an asset loader.</exception>
        public float Progress => AssetHelper<AssetAsync<T>>.GetAssetLoader(this).Progress;
    }
}