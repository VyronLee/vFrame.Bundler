// ------------------------------------------------------------
//         File: AssetLoader.cs
//        Brief: Abstract asset loader base; binds path/type/load type from the request context
//                     and exposes loaded object(s).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:44:23
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Abstract base for asset loaders: binds the asset request context and exposes the loaded object(s).
    /// </summary>
    internal abstract class AssetLoader : Loader
    {
        /// <summary>
        ///     Initializes the loader from the given bundler and request contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-level contexts passed to the base loader.</param>
        /// <param name="loaderContexts">Request context supplying the asset path, type and load type.</param>
        protected AssetLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

            AssetLoadType = loaderContexts.AssetLoadType;
            AssetPath = loaderContexts.AssetPath;
            AssetType = loaderContexts.AssetType;
            AssetLoadKey = (AssetPath, AssetType);
        }

        [JsonSerializableProperty]
        internal string AssetPath { get; }
        [JsonSerializableProperty]
        internal Type AssetType { get; }
        /// <summary>
        ///     Identity key (path, type) used to index and deduplicate asset loaders.
        /// </summary>
        internal AssetLoadKey AssetLoadKey { get; }

        /// <summary>
        ///     Requested load mode controlling how the container content is loaded.
        /// </summary>
        internal AssetLoadType AssetLoadType { get; }

        /// <summary>
        ///     The main loaded asset, or null when loading failed or nothing was loaded.
        /// </summary>
        public abstract Object AssetObject { get; }

        /// <summary>
        ///     All loaded assets, including sub-assets or all assets when requested.
        /// </summary>
        public abstract Object[] AssetObjects { get; }
    }
}