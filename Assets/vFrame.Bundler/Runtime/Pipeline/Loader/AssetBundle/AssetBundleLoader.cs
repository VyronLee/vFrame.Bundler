// ------------------------------------------------------------
//         File: AssetBundleLoader.cs
//        Brief: Base for loaders driven by one AssetBundle: owns the bundle path and the adapter
//               that creates the underlying AssetBundle instance.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:25:20
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Base implementation for loaders that are driven by a single AssetBundle.
    /// Holds the bundle path and the adapter used to create the underlying AssetBundle instance.
    /// </summary>
    internal abstract class AssetBundleLoader : Loader
    {
        /// <summary>
        /// Initializes the loader with the given bundle path and resolves the bundle creation adapter
        /// from bundler options, falling back to the built-in adapter when none is configured.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler state and options.</param>
        /// <param name="loaderContexts">Per-load context for this loader.</param>
        /// <param name="bundlePath">Path of the AssetBundle this loader drives.</param>
        protected AssetBundleLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts, string bundlePath)
            : base(bundlerContexts, loaderContexts)
        {

            BundlePath = bundlePath;
            Adapter = bundlerContexts.Options.AssetBundleCreateAdapter ??
                       new InternalAssetBundleCreateAdapter(bundlerContexts);
        }

        /// <summary>Path of the AssetBundle this loader drives (serialized for diagnostics).</summary>
        [JsonSerializableProperty]
        public string BundlePath { get; }

        /// <summary>Adapter that creates and opens the underlying AssetBundle handle.</summary>
        protected IAssetBundleCreateAdapter Adapter { get; }

        /// <summary>The AssetBundle instance opened by this loader.</summary>
        public abstract AssetBundle AssetBundle { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, BundlePath: {BundlePath}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}