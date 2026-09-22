// ------------------------------------------------------------
//         File: Asset.cs
//        Brief: Reference-counted sync asset handles (Asset, Asset<T>) wrapping a loaded loader;
//               Unload releases the load retain; GetRawAsset/Instantiate/SetTo expose the asset.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:20:14
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    /// Reference-counted handle to a synchronously loaded asset. Retains the underlying loader on
    /// load; consume the asset via <see cref="GetRawAsset"/>, <see cref="Instantiate"/>, or
    /// <see cref="SetTo{TComponent,TLink,TProxy}"/>, and balance the load retain with
    /// <see cref="Unload"/>.
    /// </summary>
    public struct Asset : ILoaderHandler
    {
        /// <summary>
        /// The loader backing this handle; retained from load until <see cref="Unload"/>.
        /// </summary>
        private Loader _loader;

        /// <summary>
        /// Sets the loader and retains it so the collector cannot reclaim it while the handle
        /// lives; the retain is balanced by <see cref="Unload"/>.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the bundler contexts this handle was created from.
        /// </summary>
        BundlerContexts ILoaderHandler.BundlerContexts { get; set; }

        /// <summary>
        /// No-op; synchronous handles require no per-frame work.
        /// </summary>
        void ILoaderHandler.Update()
        {

        }

        /// <summary>
        /// Marks the handle unloaded and releases the load-time retain.
        /// </summary>
        /// <returns>Always <see cref="UnloadOperation.Completed"/>; unloading is synchronous.</returns>
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

        /// <summary>
        /// Gets a value indicating whether this handle has been unloaded.
        /// </summary>
        [JsonSerializableProperty]
        public bool IsUnloaded { get; private set; }

        /// <summary>
        /// Gets the underlying raw asset object without extra type safety.
        /// </summary>
        /// <returns>The raw <see cref="Object"/> held by the loader.</returns>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public Object GetRawAsset()
        {
            return AssetHelper<Asset>.GetRawAsset(this);
        }

        /// <summary>
        /// Gets all raw objects held by the loader (e.g. every sub-asset).
        /// </summary>
        /// <returns>The raw <see cref="Object"/> array held by the loader.</returns>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public Object[] GetAllRawAssets()
        {
            return AssetHelper<Asset>.GetAllRawAssets(this);
        }

        /// <summary>
        /// Instantiates the loaded asset as a new scene object.
        /// </summary>
        /// <param name="parent">Optional parent transform for the instance.</param>
        /// <param name="stayWorldPosition">Keeps world position when parented.</param>
        /// <returns>The instantiated object.</returns>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public Object Instantiate(Transform parent = null, bool stayWorldPosition = false)
        {
            return AssetHelper<Asset>.Instantiate(this, parent, stayWorldPosition);
        }

        /// <summary>
        /// Binds the asset's references onto a component through the property-link system.
        /// </summary>
        /// <typeparam name="TComponent">Target component type.</typeparam>
        /// <typeparam name="TLink">Link asset type referenced by the component.</typeparam>
        /// <typeparam name="TProxy">Property-link proxy implementation.</typeparam>
        /// <param name="target">Component receiving the property links.</param>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public void SetTo<TComponent, TLink, TProxy>(TComponent target)
            where TComponent : Component
            where TLink : Object
            where TProxy : PropertyLink<TComponent, TLink>, new()
        {

            AssetHelper<Asset>.SetTo<TComponent, TLink, TProxy>(this, target);
        }

        /// <summary>
        /// Adds a strong reference to the underlying loader, extending its lifetime.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public void Retain()
        {
            AssetHelper<Asset>.GetAssetLoader(this).Retain();
        }

        /// <summary>
        /// Releases one strong reference from the underlying loader.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public void Release()
        {
            AssetHelper<Asset>.GetAssetLoader(this).Release();
        }
    }

    /// <summary>
    /// Typed reference-counted handle to a synchronously loaded asset of type
    /// <typeparamref name="T"/>. Behavior mirrors <see cref="Asset"/> with typed accessors.
    /// </summary>
    /// <typeparam name="T">Asset object type.</typeparam>
    public struct Asset<T> : ILoaderHandler where T : Object
    {
        /// <summary>
        /// The loader backing this handle; retained from load until <see cref="Unload"/>.
        /// </summary>
        private Loader _loader;

        /// <summary>
        /// Sets the loader and retains it so the collector cannot reclaim it while the handle
        /// lives; the retain is balanced by <see cref="Unload"/>.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the bundler contexts this handle was created from.
        /// </summary>
        BundlerContexts ILoaderHandler.BundlerContexts { get; set; }

        /// <summary>
        /// No-op; synchronous handles require no per-frame work.
        /// </summary>
        void ILoaderHandler.Update()
        {

        }

        /// <summary>
        /// Marks the handle unloaded and releases the load-time retain.
        /// </summary>
        /// <returns>Always <see cref="UnloadOperation.Completed"/>; unloading is synchronous.</returns>
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

        /// <summary>
        /// Gets a value indicating whether this handle has been unloaded.
        /// </summary>
        [JsonSerializableProperty]
        public bool IsUnloaded { get; private set; }

        /// <summary>
        /// Gets the underlying raw asset object as <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The raw asset cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public T GetRawAsset()
        {
            return AssetHelper<Asset<T>>.GetRawAsset(this) as T;
        }

        /// <summary>
        /// Gets all raw objects held by the loader (e.g. every sub-asset) as <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The raw objects cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public T[] GetAllRawAssets()
        {
            return AssetHelper<Asset<T>>.GetAllRawAssets(this) as T[];
        }

        /// <summary>
        /// Instantiates the loaded asset as a new scene object.
        /// </summary>
        /// <param name="parent">Optional parent transform for the instance.</param>
        /// <param name="stayWorldPosition">Keeps world position when parented.</param>
        /// <returns>The instantiated object cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public T Instantiate(Transform parent = null, bool stayWorldPosition = false)
        {
            return AssetHelper<Asset<T>>.Instantiate(this, parent, stayWorldPosition) as T;
        }

        /// <summary>
        /// Binds the asset's references onto a component through the property-link system.
        /// </summary>
        /// <typeparam name="TComponent">Target component type.</typeparam>
        /// <typeparam name="TProxy">Property-link proxy implementation for <typeparamref name="T"/>.</typeparam>
        /// <param name="target">Component receiving the property links.</param>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public void SetTo<TComponent, TProxy>(TComponent target)
            where TComponent : Component
            where TProxy : PropertyLink<TComponent, T>, new()
        {

            AssetHelper<Asset<T>>.SetTo<TComponent, T, TProxy>(this, target);
        }

        /// <summary>
        /// Adds a strong reference to the underlying loader, extending its lifetime.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public void Retain()
        {
            AssetHelper<Asset<T>>.GetAssetLoader(this).Retain();
        }

        /// <summary>
        /// Releases one strong reference from the underlying loader.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the handle has no valid loader.</exception>
        public void Release()
        {
            AssetHelper<Asset<T>>.GetAssetLoader(this).Release();
        }
    }
}