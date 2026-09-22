// ------------------------------------------------------------
//         File: ResourcesAssetLoaderSync.cs
//        Brief: Synchronously loads one asset via Resources.Load, mapping the project path to a Resources path.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:55:50
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Loads a single asset synchronously from a Resources folder via <see cref="Resources.Load(string, System.Type)"/>.
    /// </summary>
    internal class ResourcesAssetLoaderSync : AssetLoader
    {
        /// <summary>The loaded asset, or null when nothing was found.</summary>
        private Object _assetObject;

        /// <summary>Single-element array wrapping <see cref="_assetObject"/> for the uniform multi-asset API.</summary>
        private Object[] _assetObjects;

        /// <summary>
        ///     Initializes the loader from the given bundler and request contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-level contexts passed to the base loader.</param>
        /// <param name="loaderContexts">Request context supplying the asset path and type.</param>
        public ResourcesAssetLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }

        /// <summary>
        ///     Reports 1 once finished; 0 in every other state.
        /// </summary>
        [JsonSerializableProperty]
        public override float Progress => IsDone ? 1f : 0f;

        /// <summary>
        ///     Converts the project-relative asset path to a Resources path, loads it synchronously and
        ///     finishes the loader, or logs an error and aborts the loader when nothing was found.
        /// </summary>
        protected override void OnStart()
        {
            var resPath = PathUtils.RelativeProjectPathToRelativeResourcesPath(AssetPath);

            var sb = StringBuilderPool.Get();
            sb.Append(Path.GetDirectoryName(resPath));
            sb.Append("/");
            sb.Append(Path.GetFileNameWithoutExtension(resPath));
            resPath = sb.ToString();
            StringBuilderPool.Return(sb);

            _assetObject = Resources.Load(resPath, AssetType);
            _assetObjects = new[] { _assetObject };
            if (_assetObject) {
                Finish();
                return;
            }

            Facade.GetSystem<LogSystem>().LogError("Load asset from Resources failed: {0}", AssetPath);
            Abort();
        }

        /// <summary>
        ///     Clears the loaded asset reference.
        /// </summary>
        protected override void OnStop()
        {
            _assetObject = null;
        }

        /// <summary>
        ///     Completes the loader immediately; loading already happened synchronously in <see cref="OnStart"/>.
        /// </summary>
        protected override void OnUpdate()
        {
            Finish();
        }

        /// <summary>
        ///     Forces the loader to complete; loading already happened synchronously in <see cref="OnStart"/>.
        /// </summary>
        protected override void OnForceComplete()
        {
            Finish();
        }

        /// <summary>
        ///     The loaded asset, or null when loading failed.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">The loader has not finished.</exception>
        public override Object AssetObject {
            get {
                ThrowIfNotFinished();
                return _assetObject;
            }
        }

        /// <summary>
        ///     Single-element array containing the loaded asset.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">The loader has not finished.</exception>
        public override Object[] AssetObjects {
            get {
                ThrowIfNotFinished();
                return _assetObjects;
            }
        }
    }
}