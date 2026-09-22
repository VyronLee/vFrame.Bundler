// ------------------------------------------------------------
//         File: AssetHelper.cs
//        Brief: Shared helpers for asset handle structs: cast to AssetLoader, fetch raw assets,
//               route Instantiate/SetTo through LinkSystem.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:20:23
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    /// Shared static helpers for asset handle structs: casts the handler's loader to
    /// <see cref="AssetLoader"/>, exposes raw asset objects, and routes instantiation and
    /// property linking through <see cref="LinkSystem"/>.
    /// </summary>
    /// <typeparam name="T">Concrete loader handler type these helpers operate on.</typeparam>
    internal static class AssetHelper<T> where T : ILoaderHandler
    {
        /// <summary>
        /// Casts the handler's loader to an <see cref="AssetLoader"/>.
        /// </summary>
        /// <param name="loaderHandler">The loader handler whose loader is cast.</param>
        /// <returns>The underlying <see cref="AssetLoader"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the loader is not an <see cref="AssetLoader"/>.</exception>
        public static AssetLoader GetAssetLoader(T loaderHandler)
        {
            return loaderHandler.Loader as AssetLoader
                   ?? throw new ArgumentException("AssetLoader expected, got: "
                       + (loaderHandler.Loader?.GetType().Name ?? "null"));
        }

        /// <summary>
        /// Gets the bundler facade from the handler's owned bundler contexts.
        /// </summary>
        /// <param name="loaderHandler">The loader handler whose contexts are read.</param>
        /// <returns>The <see cref="Bundler"/> facade.</returns>
        public static Bundler GetFacade(T loaderHandler)
        {
            return loaderHandler.BundlerContexts.Bundler;
        }

        /// <summary>
        /// Gets the primary asset object loaded by the handler.
        /// </summary>
        /// <param name="loaderHandler">The loader handler whose asset is read.</param>
        /// <returns>The main loaded <see cref="Object"/>, or null when nothing is loaded yet.</returns>
        /// <exception cref="ArgumentException">Thrown when the loader is not an <see cref="AssetLoader"/>.</exception>
        public static Object GetRawAsset(T loaderHandler)
        {
            return GetAssetLoader(loaderHandler).AssetObject;
        }

        /// <summary>
        /// Gets all asset objects loaded by the handler, including sub-assets.
        /// </summary>
        /// <param name="loaderHandler">The loader handler whose assets are read.</param>
        /// <returns>All loaded <see cref="Object"/> instances.</returns>
        /// <exception cref="ArgumentException">Thrown when the loader is not an <see cref="AssetLoader"/>.</exception>
        public static Object[] GetAllRawAssets(T loaderHandler)
        {
            return GetAssetLoader(loaderHandler).AssetObjects;
        }

        /// <summary>
        /// Instantiates the handler's main asset and registers it with the
        /// <see cref="LinkSystem"/> so property links keep working on the copy.
        /// </summary>
        /// <param name="loaderHandler">The loader handler owning the asset to instantiate.</param>
        /// <param name="parent">Parent transform for the instance, or null for no parent.</param>
        /// <param name="stayWorldPosition">Keeps the instance's world position when parented.</param>
        /// <returns>The linked instance of the asset.</returns>
        public static Object Instantiate(T loaderHandler, Transform parent = null, bool stayWorldPosition = false)
        {
            var proxySystem = GetFacade(loaderHandler).GetSystem<LinkSystem>();
            return proxySystem.InstantiateAndLink(loaderHandler, parent, stayWorldPosition);
        }

        /// <summary>
        /// Re-links a component property on an existing instance through the
        /// <see cref="LinkSystem"/>, so it tracks the handler's source asset object.
        /// </summary>
        /// <typeparam name="TComponent">Component type owning the property.</typeparam>
        /// <typeparam name="TObject">Asset object type assigned to the property.</typeparam>
        /// <typeparam name="TLink">Property link type binding the component property to the object.</typeparam>
        /// <param name="loaderHandler">The loader handler owning the source asset object.</param>
        /// <param name="target">Target component whose property is re-linked.</param>
        public static void SetTo<TComponent, TObject, TLink>(T loaderHandler, TComponent target)
            where TComponent : Component
            where TObject : Object
            where TLink : PropertyLink<TComponent, TObject>, new()
        {

            var proxySystem = GetFacade(loaderHandler).GetSystem<LinkSystem>();
            proxySystem.RelinkProperty<TComponent, TObject, TLink>(loaderHandler, target);
        }
    }
}