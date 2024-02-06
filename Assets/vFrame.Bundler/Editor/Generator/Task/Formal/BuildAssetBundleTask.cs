// ------------------------------------------------------------
//         File: BunildAssetBundleTask.cs
//        Brief: BunildAssetBundleTask.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:42
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace vFrame.Bundler.Task.Formal
{
    internal class BuildAssetBundleTask : BuildTaskBase
    {
        public override void Run(BuildContext context) {
            var outputPath = context.BuildSettings.BundlePath;
            var options = context.BuildSettings.AssetBundleBuildOptions;
            var buildTarget = context.BuildSettings.BuildTarget;
            var builds = context.BundleInfos.Select(BundleInfoToBundleBuild).ToArray();

            if (!Directory.Exists(outputPath)) {
                Directory.CreateDirectory(outputPath);
            }

            if (context.BuildSettings.DryRun) {
                options |= BuildAssetBundleOptions.DryRunBuild;
            }
            context.AssetBundleManifest = BuildPipeline.BuildAssetBundles(outputPath, builds, options, buildTarget);
        }

        private AssetBundleBuild BundleInfoToBundleBuild(KeyValuePair<string, BundleInfo> kv) {
            var bundleInfo = kv.Value;
            var build = new AssetBundleBuild {
                assetBundleName = bundleInfo.BundlePath,
                assetNames = bundleInfo.AssetPaths.ToArray()
            };
            return build;
        }
    }
}