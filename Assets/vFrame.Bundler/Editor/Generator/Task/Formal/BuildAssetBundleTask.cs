// ------------------------------------------------------------
//         File: BuildAssetBundleTask.cs
//        Brief: Pipeline step 5: converts BundleInfos into AssetBundleBuild entries and invokes
//               Unity BuildPipeline.BuildAssetBundles, honoring the DryRun build option.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-10-17 10:05:21
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Build pipeline task that writes AssetBundle files to the output directory
    ///     using Unity's build pipeline, and stores the resulting manifest in the context.
    /// </summary>
    internal class BuildAssetBundleTask : BuildTaskBase
    {
        /// <summary>
        ///     Converts collected bundle infos into Unity build entries and runs
        ///     <see cref="BuildPipeline.BuildAssetBundles" /> for the configured target.
        /// </summary>
        /// <param name="context">Build context carrying settings, bundle infos, and the output manifest.</param>
        /// <exception cref="BundleException">Thrown when no bundles were collected or Unity returns no manifest.</exception>
        public override void Run(BuildContext context)
        {
            var outputPath = context.BuildSettings.BundlePath;
            var options = context.BuildSettings.AssetBundleBuildOptions;
            var buildTarget = context.BuildSettings.BuildTarget;
            var builds = context.BundleInfos.Select(BundleInfoToBundleBuild).ToArray();

            if (builds.Length == 0) {
                var searchPaths = string.Join(", ", context.BuildRules.MainRules.Select(rule => rule.SearchPath));
                throw new BundleException(
                    $"AssetBundle build collected 0 bundles from {context.BuildRules.MainRules.Count} main rule(s). " +
                    $"Search paths: [{searchPaths}]. Check SearchPath, Include/Exclude filters, and asset import errors.");
            }

            if (!Directory.Exists(outputPath)) {
                Directory.CreateDirectory(outputPath);
            }

            if (context.BuildSettings.DryRun) {
                options |= BuildAssetBundleOptions.DryRunBuild;
            }
            context.AssetBundleManifest = BuildPipeline.BuildAssetBundles(outputPath, builds, options, buildTarget);
            if (context.AssetBundleManifest == null) {
                throw new BundleException(
                    $"AssetBundle build failed: Unity returned no manifest for {builds.Length} bundle(s), " +
                    $"target {buildTarget}, output '{outputPath}'. See the Unity console for build errors.");
            }
        }

        /// <summary>
        ///     Maps one bundle info to Unity's <see cref="AssetBundleBuild" /> descriptor.
        /// </summary>
        /// <param name="kv">Key/value pair of bundle name and its analyzed info.</param>
        /// <returns>Build descriptor with the bundle file name and its asset list.</returns>
        private AssetBundleBuild BundleInfoToBundleBuild(KeyValuePair<string, BundleInfo> kv)
        {
            var bundleInfo = kv.Value;
            var build = new AssetBundleBuild {
                assetBundleName = bundleInfo.BundlePath,
                assetNames = bundleInfo.AssetPaths.ToArray()
            };
            return build;
        }
    }
}