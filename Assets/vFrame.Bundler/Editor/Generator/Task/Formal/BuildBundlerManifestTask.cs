// ------------------------------------------------------------
//         File: BuildBundlerManifestTask.cs
//        Brief: Formal build step 7: populates the bundler manifest from build context results and
//               writes it to disk as JSON.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:43:52
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Formal build task (step 7): populates the bundler manifest from build results and writes it to
    ///     disk as JSON, storing the result in <see cref="BuildContext.BundlerManifest" />.
    /// </summary>
    internal class BuildBundlerManifestTask : BuildTaskBase
    {
        /// <summary>
        ///     Builds the manifest: grants asset-to-bundle mappings and bundle dependency sets from the
        ///     build context, writes the JSON manifest to disk, and stores it in the context.
        /// </summary>
        /// <param name="context">Build context supplying build results and receiving the manifest.</param>
        public override void Run(BuildContext context)
        {
            var manifest = new BundlerManifest();
            GrantAssetInfos(context, manifest);
            GrantAssetBundleInfos(context, manifest);
            WriteToDisk(context, manifest);
            context.BundlerManifest = manifest;
        }

        /// <summary>
        ///     Maps each main asset path to its owning bundle path in the manifest, with a progress bar.
        /// </summary>
        /// <param name="context">Build context supplying the analyzed main-asset infos.</param>
        /// <param name="manifest">Manifest whose asset map is populated.</param>
        private void GrantAssetInfos(BuildContext context, BundlerManifest manifest)
        {
            var index = 0f;
            var total = context.MainAssetInfos.Count;
            try {
                foreach (var kv in context.MainAssetInfos) {
                    var assetInfo = kv.Value;
                    EditorUtility.DisplayProgressBar("Building Bundler Manifest",
                        $"Granting asset info: {assetInfo.AssetPath}", ++index / total);
                    manifest.Assets[assetInfo.AssetPath] = assetInfo.BundlePath;
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        ///     Maps each built bundle to its dependency set in the manifest, with a progress bar.
        /// </summary>
        /// <param name="context">Build context supplying bundle infos and the AssetBundle dependency manifest.</param>
        /// <param name="manifest">Manifest whose bundle dependency map is populated.</param>
        private void GrantAssetBundleInfos(BuildContext context, BundlerManifest manifest)
        {
            var index = 0f;
            var total = context.MainAssetInfos.Count;
            var abs = context.BundleInfos.Keys;

            try {
                foreach (var ab in abs) {
                    EditorUtility.DisplayProgressBar("Building Bundler Manifest",
                        $"Granting assetBundle: {ab}", ++index / total);

                    var dependencies = context.AssetBundleManifest.GetAllDependencies(ab) ?? Array.Empty<string>();
                    manifest.Bundles[ab] = new BundleDependencySet(dependencies);
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        ///     Serializes the manifest to JSON and writes it into the bundle output directory under the
        ///     configured manifest file name, creating the directory if missing.
        /// </summary>
        /// <param name="context">Build context supplying bundle output path and manifest file name.</param>
        /// <param name="manifest">Manifest to serialize.</param>
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