// ------------------------------------------------------------
//         File: AssetBundleAssetLoaderSync.cs
//        Brief: Loads assets (or sub-assets) synchronously from the group's AssetBundle and finishes immediately.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:37:40
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    /// Synchronous asset loader for AssetBundle mode. Loads the requested asset (or sub-assets)
    /// directly from the parent <see cref="AssetBundleLoaderGroup"/>'s AssetBundle in
    /// <see cref="OnStart"/> and finishes immediately; no per-frame progress tracking.
    /// </summary>
    internal class AssetBundleAssetLoaderSync : AssetBundleAssetLoader
    {
        /// <summary>
        /// Main loaded asset, or the first sub-asset when loading with sub-assets;
        /// null until <see cref="OnStart"/> completes and reset on stop.
        /// </summary>
        private Object _assetObject;

        /// <summary>
        /// All loaded assets; wraps <see cref="_assetObject"/> in a single-element array
        /// when loading without sub-assets.
        /// </summary>
        private Object[] _assetObjects;

        /// <summary>
        /// Creates a new synchronous AssetBundle-mode asset loader.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-level contexts.</param>
        /// <param name="loaderContexts">Loader contexts; <see cref="LoaderContexts.ParentLoader"/>
        /// must be an <see cref="AssetBundleLoaderGroup"/>.</param>
        /// <exception cref="BundleArgumentException">Thrown when the parent loader is not
        /// an <see cref="AssetBundleLoaderGroup"/>.</exception>
        public AssetBundleAssetLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }

        /// <summary>
        /// Loading progress: 0 until the loader is done, then 1 (synchronous load has no
        /// intermediate progress).
        /// </summary>
        [JsonSerializableProperty]
        public override float Progress => IsDone ? 1f : 0f;

        /// <summary>
        /// Loads the asset (or sub-assets) from the group's AssetBundle and finishes or aborts
        /// synchronously; failures are reported via error logging and <c>Abort</c> rather than
        /// exceptions.
        /// </summary>
        protected override void OnStart()
        {
            var assetBundle = BundleLoader.AssetBundle;
            if (!assetBundle) {
                Abort();
                return;
            }

            switch (AssetLoadType) {
                case AssetLoadType.LoadAsset:
                    _assetObject = assetBundle.LoadAsset(AssetPath, AssetType);
                    _assetObjects = new[] { _assetObject };
                    break;
                case AssetLoadType.LoadAssetWithSubAsset:
                    _assetObjects = assetBundle.LoadAssetWithSubAssets(AssetPath, AssetType);
                    if (_assetObjects.Length > 0) {
                        _assetObject = _assetObjects[0];
                    }
                    break;
                default:
                    Facade.GetSystem<LogSystem>().LogError("Unsupported load type: {0}", AssetLoadType);
                    break;
            }

            if (_assetObject) {
                Finish();
                return;
            }

            Facade.GetSystem<LogSystem>().LogError("Load asset from AssetBundle failed: {0}, type: {1}",
                AssetPath, AssetType);

            Abort();
        }

        /// <summary>
        /// Releases references to the loaded asset(s) without unloading the AssetBundle itself.
        /// </summary>
        protected override void OnStop()
        {
            _assetObject = null;
            _assetObjects = null;
        }

        /// <summary>
        /// No-op update; the load completed synchronously in <see cref="OnStart"/>.
        /// </summary>
        protected override void OnUpdate()
        {
            Finish();
        }

        /// <summary>
        /// Forces the loader into the finished state; redundant after a synchronous load.
        /// </summary>
        protected override void OnForceComplete()
        {
            Finish();
        }

        /// <summary>
        /// The main loaded asset.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">Thrown when the loader has
        /// not finished.</exception>
        public override Object AssetObject {
            get {
                ThrowIfNotFinished();
                return _assetObject;
            }
        }

        /// <summary>
        /// All loaded assets (main plus sub-assets).
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">Thrown when the loader has
        /// not finished.</exception>
        public override Object[] AssetObjects {
            get {
                ThrowIfNotFinished();
                return _assetObjects;
            }
        }
    }
}