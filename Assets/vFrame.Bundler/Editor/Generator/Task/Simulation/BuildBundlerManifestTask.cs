// ------------------------------------------------------------
//         File: BuildBundlerManifestTask.cs
//        Brief: Simulation build step 2: writes the bundler manifest with only the asset-to-bundle mapping
//               (no real bundles built) to disk as JSON.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:57:56
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.IO;
using UnityEngine;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Simulation build task (step 2): creates a bundler manifest containing only the main-asset to
    ///     bundle mapping, writes it to disk as JSON without building real AssetBundle files, and stores
    ///     it in <see cref="BuildContext.BundlerManifest" />.
    /// </summary>
    internal class SimulationBuildBundlerManifestTask : BuildTaskBase
    {
        /// <summary>
        ///     Builds the manifest: grants the asset-to-bundle mappings from the build context, writes the
        ///     JSON manifest to disk, and stores it in the context.
        /// </summary>
        /// <param name="context">Build context supplying the analyzed main assets and receiving the manifest.</param>
        public override void Run(BuildContext context)
        {
            var manifest = new BundlerManifest();
            GrantAssetInfos(context, manifest);
            WriteToDisk(context, manifest);
            context.BundlerManifest = manifest;
        }

        /// <summary>
        ///     Maps each analyzed main asset path to its owning bundle path in the manifest.
        /// </summary>
        /// <param name="context">Build context supplying the analyzed main-asset infos.</param>
        /// <param name="manifest">Manifest whose asset map is populated.</param>
        private void GrantAssetInfos(BuildContext context, BundlerManifest manifest)
        {
            foreach (var kv in context.MainAssetInfos) {
                var assetInfo = kv.Value;
                manifest.Assets[assetInfo.AssetPath] = assetInfo.BundlePath;
            }
        }

        /// <summary>
        ///     Serializes the manifest to JSON and writes it to the bundle output directory, creating the
        ///     directory when it does not exist.
        /// </summary>
        /// <param name="context">Build context supplying the output path settings.</param>
        /// <param name="manifest">Manifest to serialize and save.</param>
        private void WriteToDisk(BuildContext context, BundlerManifest manifest)
        {
            var jsonData = JsonUtility.ToJson(manifest);
            var savePath = PathUtils.Combine(
                context.BuildSettings.BundlePath,
                context.BuildSettings.ManifestFileName);

            if (!Directory.Exists(context.BuildSettings.BundlePath)) {
                Directory.CreateDirectory(context.BuildSettings.BundlePath);
            }
            File.WriteAllText(savePath, jsonData);
        }
    }
}