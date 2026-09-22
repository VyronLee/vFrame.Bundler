// ------------------------------------------------------------
//         File: AssetBundleLoaderSync.cs
//        Brief: Synchronous single-bundle loader: opens the AssetBundle once via the adapter on start,
//               finishes immediately, and unloads it with all its assets on stop.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:37:49
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Loads a single AssetBundle synchronously: the bundle is opened once on start, progress is
    /// either 0 or 1, and the bundle is unloaded with all its assets on stop.
    /// </summary>
    internal class AssetBundleLoaderSync : AssetBundleLoader
    {
        /// <summary>The AssetBundle opened during load; null before the bundle is created or after it is unloaded.</summary>
        private AssetBundle _assetBundle;

        /// <inheritdoc cref="AssetBundleLoader(BundlerContexts, LoaderContexts, string)"/>
        public AssetBundleLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts, string bundlePath)
            : base(bundlerContexts, loaderContexts, bundlePath)
        {

        }

        /// <summary>Progress of the load: 1 once done, otherwise 0 (no incremental progress for synchronous loads).</summary>
        [JsonSerializableProperty]
        public override float Progress => IsDone ? 1f : 0f;

        /// <summary>
        /// Creates the AssetBundle through the adapter. Finishes on success; on failure (null bundle
        /// or adapter exception) logs the error and aborts the load.
        /// </summary>
        protected override void OnStart()
        {
            try {
                _assetBundle = Adapter.CreateAssetBundle(BundlePath);
                if (_assetBundle) {
                    Finish();
                    return;
                }
            }
            catch (System.Exception e) {
                Facade.GetSystem<LogSystem>().LogException(e);
                Abort();
                return;
            }

            Facade.GetSystem<LogSystem>().LogError("Load AssetBundle failed: {0}", BundlePath);
            Abort();
        }

        /// <summary>Unloads the loaded AssetBundle together with all its assets, if one was created.</summary>
        protected override void OnStop()
        {
            if (_assetBundle) {
                _assetBundle.Unload(true);
            }
            _assetBundle = null;
        }

        /// <summary>No per-frame work needed; marks the load finished immediately.</summary>
        protected override void OnUpdate()
        {
            Finish();
        }

        /// <summary>Forces the load to complete; the load is already synchronous, so this just finishes it.</summary>
        protected override void OnForceComplete()
        {
            Finish();
        }

        /// <summary>The opened AssetBundle, or null before the load starts or after the bundle is unloaded.</summary>
        public override AssetBundle AssetBundle => _assetBundle;
    }
}