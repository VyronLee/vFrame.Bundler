// ------------------------------------------------------------
//         File: InternalAssetBundleCreateAdapter.cs
//        Brief: Default IAssetBundleCreateAdapter: resolves bundle files against ordered SearchPaths
//               and loads them synchronously or asynchronously via AssetBundle.LoadFromFile(Async).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:29:26
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

namespace vFrame.Bundler
{
    /// <summary>
    /// Default <see cref="IAssetBundleCreateAdapter"/> implementation.
    /// Resolves a bundle-relative path against the configured search paths in order and
    /// loads the first match via <see cref="AssetBundle.LoadFromFile(string)"/> (or its async variant).
    /// </summary>
    internal class InternalAssetBundleCreateAdapter : BundlerObject, IAssetBundleCreateAdapter
    {
        /// <summary>
        /// Initializes a new instance bound to the given bundler contexts.
        /// </summary>
        /// <param name="bundlerContexts">Owning bundler contexts providing options and facade access.</param>
        public InternalAssetBundleCreateAdapter(BundlerContexts bundlerContexts) : base(bundlerContexts)
        {

        }

        /// <summary>
        /// Called when the adapter is destroyed. No managed resources need explicit release.
        /// </summary>
        protected override void OnDestroy()
        {

        }

        /// <summary>
        /// Creates an asynchronous load request for the bundle file resolved from the search paths.
        /// </summary>
        /// <param name="bundlePath">Bundle-relative path of the AssetBundle file.</param>
        /// <returns>
        /// A pending <see cref="AssetBundleCreateRequest"/> for the first candidate that yields a request,
        /// or null if every candidate is missing or fails to produce a load request.
        /// </returns>
        public AssetBundleCreateRequest CreateRequest(string bundlePath)
        {
            Facade.GetSystem<LogSystem>().LogDebug("Create AssetBundleCreateRequest: {0}", bundlePath);

            var searchPaths = BundlerContexts.Options.SearchPaths;
            foreach (var basePath in searchPaths) {
                var path = PathUtils.Combine(basePath, bundlePath);
                if (PathUtils.IsFileInPersistentDataPath(path) && !File.Exists(path)) {
                    continue;
                }

                Profiler.BeginSample("InternalAssetBundleCreateRequestAdapter - LoadFromFileAsync");
                var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);
                Profiler.EndSample();

                if (bundleLoadRequest == null) {
                    continue;
                }
                Facade.GetSystem<LogSystem>().LogDebug("AssetBundleCreateRequest created: {0}", path);
                return bundleLoadRequest;
            }
            return null;
        }

        /// <summary>
        /// Loads the bundle file resolved from the search paths synchronously.
        /// </summary>
        /// <param name="bundlePath">Bundle-relative path of the AssetBundle file.</param>
        /// <returns>
        /// The loaded <see cref="AssetBundle"/> from the first candidate that succeeds,
        /// or null if every candidate is missing or fails to load.
        /// </returns>
        public AssetBundle CreateAssetBundle(string bundlePath)
        {
            Facade.GetSystem<LogSystem>().LogDebug("Load AssetBundle: {0}", bundlePath);

            var searchPaths = BundlerContexts.Options.SearchPaths;
            foreach (var basePath in searchPaths) {
                var path = PathUtils.Combine(basePath, bundlePath);
                if (PathUtils.IsFileInPersistentDataPath(path) && !File.Exists(path)) {
                    continue;
                }

                Profiler.BeginSample("InternalAssetBundleCreateRequestAdapter - LoadFromFile");
                var assetBundle = AssetBundle.LoadFromFile(path);
                Profiler.EndSample();

                if (assetBundle == null) {
                    continue;
                }
                Facade.GetSystem<LogSystem>().LogDebug("AssetBundle loaded: {0}", path);
                return assetBundle;
            }
            return null;
        }
    }
}