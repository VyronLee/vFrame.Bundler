// ------------------------------------------------------------
//         File: BuildSimulationBundlerManifestTask.cs
//        Brief: BuildSimulationBundlerManifestTask.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-5 19:16
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.IO;
using UnityEngine;

namespace vFrame.Bundler.Task.Simulation
{
    internal class BuildBundlerManifestTask : BuildTaskBase
    {
        public override void Run(BuildContext context) {
            var manifest = new BundlerManifest();
            GrantAssetInfos(context, manifest);
            WriteToDisk(context, manifest);
            context.BundlerManifest = manifest;
        }

        private void GrantAssetInfos(BuildContext context, BundlerManifest manifest) {
            foreach (var kv in context.MainAssetInfos) {
                var assetInfo = kv.Value;
                manifest.Assets[assetInfo.AssetPath] = assetInfo.BundlePath;
            }
        }

        private void WriteToDisk(BuildContext context, BundlerManifest manifest) {
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