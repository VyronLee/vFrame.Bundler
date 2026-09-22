// ------------------------------------------------------------
//         File: AnalyzePackByTopDirectoryRule.cs
//        Brief: Simulation analyzer: registers a PackByTopDirectory rule's top-directory
//               assets as main assets of the <AssetDatabase> pseudo bundle.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:07:33
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using System.IO;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Simulation analyzer that registers assets matched by a <see cref="MainBundleRule"/> of pack type
    ///     <c>PackByTopDirectory</c> and located directly under the rule's SearchPath as main assets of the
    ///     "&lt;AssetDatabase&gt;" pseudo bundle; deeper assets are skipped.
    /// </summary>
    internal class SimulationAnalyzePackByTopDirectoryRule : SimulationMainAssetAnalyzerBase
    {
        /// <summary>
        ///     Enumerates the assets matched by <paramref name="rule"/>, yielding per-asset progress and
        ///     registering only assets located directly under the rule's SearchPath as main assets of the
        ///     "&lt;AssetDatabase&gt;" pseudo bundle.
        /// </summary>
        /// <param name="context">Build context collecting the registered main asset infos.</param>
        /// <param name="rule">Main bundle rule whose top-directory assets are mapped to the pseudo bundle.</param>
        /// <returns>
        ///     A sequence of (asset path, progress) pairs whose progress rises from <c>1/N</c> to <c>1</c> as the
        ///     assets are enumerated; empty when no asset matches the rule.
        /// </returns>
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule)
        {
            var assets = FindAssets(rule);
            var index = 0f;
            var total = assets.Count;
            foreach (var asset in assets) {
                yield return (asset, ++index / total);

                if (!IsTopDirectoryAsset(asset)) {
                    continue;
                }
                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                    BundlePath = "<AssetDatabase>"
                };
                SafeAddMainAssetInfo(context, assetInfo);
            }
            yield break;

            /// <summary>
            ///     Determines whether the asset resides directly in the rule's SearchPath (its parent directory
            ///     equals the SearchPath), meaning it belongs to the "&lt;AssetDatabase&gt;" pseudo bundle.
            /// </summary>
            /// <param name="path">Asset path to test.</param>
            /// <returns><c>true</c> if the asset's containing directory equals the rule's SearchPath.</returns>
            bool IsTopDirectoryAsset(string path)
            {
                var dirName = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dirName)) {
                    return false;
                }
                return dirName == rule.SearchPath;
            }
        }
    }
}