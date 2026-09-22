// ------------------------------------------------------------
//         File: AssetDatabaseAssetLoaderSync.cs
//        Brief: Synchronously loads an asset or all its sub-assets via UnityEditor.AssetDatabase in the
//               editor, and aborts the load when running outside the editor.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:44:14
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Loader that synchronously loads an asset through <see cref="UnityEditor.AssetDatabase"/>.
    /// </summary>
    /// <remarks>
    ///     Supports loading a single asset, an asset with sub-assets, or all assets at the path.
    ///     Only functional in the Unity editor; the load aborts in runtime builds.
    /// </remarks>
    internal class AssetDatabaseAssetLoaderSync : AssetLoader
    {
        /// <summary>The primary asset object loaded at <see cref="AssetPath"/>, or the first of the loaded assets.</summary>
        private Object _assetObject;

        /// <summary>All assets loaded at <see cref="AssetPath"/> when the load type is <see cref="AssetLoadType.LoadAllAssets"/>; otherwise null.</summary>
        private Object[] _assetObjects;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AssetDatabaseAssetLoaderSync"/> class.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-wide contexts.</param>
        /// <param name="loaderContexts">Contexts describing the asset to load.</param>
        public AssetDatabaseAssetLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {
        }

        /// <summary>
        ///     Gets the load progress. Loading via AssetDatabase is synchronous, so this is either 0 or 1.
        /// </summary>
        [JsonSerializableProperty]
        public override float Progress => IsDone ? 1f : 0f;

        /// <summary>
        ///     Loads the asset from AssetDatabase according to <see cref="AssetLoadType"/>, finishing the loader
        ///     on success and aborting it on failure or in runtime builds.
        /// </summary>
        protected override void OnStart()
        {
#if UNITY_EDITOR
            switch (AssetLoadType) {
                case AssetLoadType.LoadAsset:
                case AssetLoadType.LoadAssetWithSubAsset:
                    _assetObject = UnityEditor.AssetDatabase.LoadAssetAtPath(AssetPath, AssetType);
                    break;
                case AssetLoadType.LoadAllAssets:
                    _assetObjects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(AssetPath);
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

            Facade.GetSystem<LogSystem>().LogError("Load asset from AssetDatabase failed: {0}", AssetPath);
            Abort();
#else
            Facade.GetSystem<LogSystem>().LogError(
                $"{nameof(AssetDatabaseAssetLoaderSync)} is not supported in runtime mode.");
            Abort();
#endif
        }

        /// <summary>
        ///     Releases the references to the loaded asset objects.
        /// </summary>
        protected override void OnStop()
        {
            _assetObject = null;
            _assetObjects = null;
        }

        /// <summary>
        ///     Completes the loader; the actual loading already happened synchronously in <see cref="OnStart"/>.
        /// </summary>
        protected override void OnUpdate()
        {
            Finish();
        }

        /// <summary>
        ///     Completes the loader immediately on forced completion.
        /// </summary>
        protected override void OnForceComplete()
        {
            Finish();
        }

        /// <summary>
        ///     Gets the primary loaded asset object.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">Thrown when the loader has not finished.</exception>
        public override Object AssetObject {
            get {
                ThrowIfNotFinished();
                return _assetObject;
            }
        }

        /// <summary>
        ///     Gets all asset objects loaded at the path; non-null only when the load type is
        ///     <see cref="AssetLoadType.LoadAllAssets"/>.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">Thrown when the loader has not finished.</exception>
        public override Object[] AssetObjects {
            get {
                ThrowIfNotFinished();
                return _assetObjects;
            }
        }

        /// <summary>
        ///     Returns a string describing the loader type, target asset path, task state, and progress.
        /// </summary>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, AssetPath: {AssetPath}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}