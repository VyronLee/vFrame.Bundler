// ------------------------------------------------------------
//         File: AssetBundleLoaderGroup.cs
//        Brief: Aggregates the main bundle loader with its dependency bundle loaders for one asset; drives their
//               lifecycle, aggregates progress, and propagates retain/release reference counting.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:25:24
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Groups the bundle loaders required to load a single asset (main bundle plus its dependency bundles)
    ///     and exposes aggregate lifecycle, progress, and reference counting over them.
    /// </summary>
    internal abstract class AssetBundleLoaderGroup : Loader
    {
        /// <summary>Cached raw <see cref="AssetBundle"/> handle; currently unused by this class.</summary>
        private AssetBundle _assetBundle;

        /// <summary>Child loaders, with the main bundle loader always at index 0; <see langword="null"/> when setup failed.</summary>
        private readonly List<AssetBundleLoader> _loaders;

        /// <summary>
        ///     Creates the group and resolves its child loaders from the manifest.
        ///     Puts the group into the error state via <see cref="Loader.Abort"/> if the asset or bundle
        ///     entry is missing from the manifest.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler contexts (manifest, loader registry, facade systems).</param>
        /// <param name="loaderContexts">Per-loader contexts (target asset path, parent loader, etc.).</param>
        protected AssetBundleLoaderGroup(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

            _loaders = CreateBundleLoaders();
            if (null != _loaders) {
                return;
            }
            Abort();
        }

        /// <summary>Creates a concrete child loader for the given bundle path (sync or async is decided by the subclass).</summary>
        /// <param name="bundlePath">Bundle path resolved from the manifest.</param>
        /// <returns>A new, unregistered <see cref="AssetBundleLoader"/> instance.</returns>
        protected abstract AssetBundleLoader CreateAssetBundleLoader(string bundlePath);

        /// <summary>Clears the borrowed child loader references; their lifetime is owned by the collect system.</summary>
        protected override void OnDestroy()
        {
            // Child loaders are borrowed from BundlerContexts and destroyed by CollectSystem.
            _loaders?.Clear();
            base.OnDestroy();
        }

        /// <summary>Gets the loader of the main bundle (first child), or <see langword="null"/> when no loaders exist.</summary>
        public AssetBundleLoader MainBundleLoader {
            get {
                if (null == _loaders || _loaders.Count <= 0) {
                    return null;
                }
                return _loaders[0];
            }
        }

        /// <summary>Gets the asset path this group was created for.</summary>
        [JsonSerializableProperty]
        public string MainAssetPath => LoaderContexts.AssetPath;

        /// <summary>Gets the path of the main bundle, or <see langword="null"/> when no loaders exist.</summary>
        [JsonSerializableProperty]
        public string MainBundlePath => MainBundleLoader?.BundlePath;

        /// <summary>Gets the loaded <see cref="AssetBundle"/> of the main bundle, blocking until loading completes.</summary>
        /// <exception cref="BundleAssetNotReadyException">Thrown if the group did not finish successfully.</exception>
        public AssetBundle AssetBundle {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return MainBundleLoader?.AssetBundle;
            }
        }

        /// <summary>
        ///     Resolves the main bundle and its dependency set from the manifest and creates a child loader for each.
        /// </summary>
        /// <returns>Child loaders with the main bundle loader first; <see langword="null"/> if a manifest entry is missing.</returns>
        private List<AssetBundleLoader> CreateBundleLoaders()
        {
            if (!BundlerContexts.Manifest.Assets.TryGetValue(LoaderContexts.AssetPath, out var mainBundle)) {
                Facade.GetSystem<LogSystem>().LogError("Bundle data not found for asset: {0}", LoaderContexts.AssetPath);
                return null;
            }

            if (!BundlerContexts.Manifest.Bundles.TryGetValue(mainBundle, out var dependencySet)) {
                Facade.GetSystem<LogSystem>().LogError("Dependency data not found for bundle: {0}", mainBundle);
                return null;
            }

            var ret = new List<AssetBundleLoader>();
            ret.Add(GetOrCreateAssetBundleLoader(mainBundle));
            ret.AddRange(dependencySet.Values.Select(GetOrCreateAssetBundleLoader));
            return ret;
        }

        /// <summary>Returns the existing shared loader for the bundle, or creates and registers a new one.</summary>
        /// <param name="bundlePath">Bundle path resolved from the manifest.</param>
        /// <returns>The shared <see cref="AssetBundleLoader"/> for <paramref name="bundlePath"/>.</returns>
        private AssetBundleLoader GetOrCreateAssetBundleLoader(string bundlePath)
        {
            if (BundlerContexts.TryGetLoader(bundlePath, out AssetBundleLoader bundleLoader)) {
                return bundleLoader;
            }
            var ret = CreateAssetBundleLoader(bundlePath);
            BundlerContexts.AddLoader(ret);
            return ret;
        }

        /// <summary>Gets the average progress across all child loaders; 1 when the group has no loaders.</summary>
        [JsonSerializableProperty]
        public override float Progress {
            get {
                if (null == _loaders) {
                    return 1f;
                }
                var ret = 0f;
                foreach (var loader in _loaders) {
                    ret += loader?.Progress ?? 0f;
                }
                return ret / _loaders.Count;
            }
        }

        /// <summary>Starts all child loaders.</summary>
        protected override void OnStart()
        {
            foreach (var loader in _loaders) {
                loader.Start();
            }
        }

        /// <summary>Stops all child loaders.</summary>
        protected override void OnStop()
        {
            foreach (var loader in _loaders) {
                loader.Stop();
            }
        }

        /// <summary>Updates all child loaders, then finishes the group when all are done or aborts it when any fails.</summary>
        protected override void OnUpdate()
        {
            var finished = true;
            var error = false;
            foreach (var loader in _loaders) {
                loader.Update();
                finished &= loader.IsDone;
                error |= loader.IsError;
            }
            if (error) {
                Abort();
            }
            else if (finished) {
                Finish();
            }
        }

        /// <summary>Force-completes all child loaders, then finishes the group when all are done or aborts it when any fails.</summary>
        protected override void OnForceComplete()
        {
            var finished = true;
            var error = false;
            foreach (var loader in _loaders) {
                loader.ForceComplete();
                finished &= loader.IsDone;
                error |= loader.IsError;
            }
            if (error) {
                Abort();
            }
            else if (finished) {
                Finish();
            }
        }

        /// <summary>Retains all child loaders, then this group.</summary>
        public override void Retain()
        {
            foreach (var loader in _loaders) {
                loader.Retain();
            }
            base.Retain();
        }

        /// <summary>Releases all child loaders, then this group.</summary>
        public override void Release()
        {
            foreach (var loader in _loaders) {
                loader.Release();
            }
            base.Release();
        }

        /// <summary>Returns a debug description with type name, main bundle path, task state, and progress.</summary>
        /// <returns>A human-readable summary string.</returns>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, MainBundlePath: {MainBundlePath}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}