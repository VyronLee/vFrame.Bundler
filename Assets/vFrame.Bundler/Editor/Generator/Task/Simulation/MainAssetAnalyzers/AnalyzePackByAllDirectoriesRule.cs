// ------------------------------------------------------------
//         File: AnalyzePackByAllDirectoriesRule.cs
//        Brief: Simulation PackByAllDirectories analyzer: registers every filtered asset as a
//               main asset in the placeholder "<AssetDatabase>" bundle.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:22:08
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Simulation analyzer for <c>PackByAllDirectories</c> rules. Registers every asset found
    ///     under the rule's search path as a main asset assigned to the placeholder
    ///     "&lt;AssetDatabase&gt;" bundle name (resolved to the real bundle by the simulation loader).
    /// </summary>
    internal class SimulationAnalyzePackByAllDirectoriesRule : SimulationMainAssetAnalyzerBase
    {
        /// <inheritdoc cref="MainAssetAnalyzerBase.OnRun"/>
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule)
        {
            var assets = FindAssets(rule);
            var index = 0f;
            var total = assets.Count;

            foreach (var asset in assets) {
                yield return (asset, ++index / total);

                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                    BundlePath = "<AssetDatabase>"
                };
                SafeAddMainAssetInfo(context, assetInfo);
            }
        }
    }
}