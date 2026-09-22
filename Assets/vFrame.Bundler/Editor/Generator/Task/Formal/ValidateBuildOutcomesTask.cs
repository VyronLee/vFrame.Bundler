// ------------------------------------------------------------
//         File: ValidateBuildOutcomesTask.cs
//        Brief: Build step 6: verify built AssetBundles match the desired set; log and fail on
//               missing or undesired bundles.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:22:08
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Linq;
using System.Text;
using UnityEngine;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Build pipeline step 6: verifies the built AssetBundle set exactly matches the desired
    ///     bundle set, logging any missing or undesired bundles and throwing on mismatch.
    /// </summary>
    internal class ValidateBuildOutcomesTask : BuildTaskBase
    {
        /// <summary>Runs the build outcome verification step.</summary>
        /// <param name="context">Shared context carrying build inputs and accumulated step outputs.</param>
        public override void Run(BuildContext context)
        {
            VerifyBundleQuantities(context);
        }

        /// <summary>
        ///     Compares the desired bundle names against the built asset bundle manifest, logging and
        ///     throwing on any difference.
        /// </summary>
        /// <param name="context">Build context providing the desired bundle names and the built manifest.</param>
        /// <exception cref="BundleException">Thrown when the build result contains missing or undesired bundles.</exception>
        private void VerifyBundleQuantities(BuildContext context)
        {
            var desired = context.BundleInfos.Keys;
            var actual = context.AssetBundleManifest.GetAllAssetBundles();

            var missing = desired.Except(actual).ToList();
            var undesired = actual.Except(desired).ToList();

            if (missing.Count <= 0 && undesired.Count <= 0) {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Missing bundles:");
            foreach (var bundle in missing) {
                sb.AppendLine($"  - {bundle}");
            }
            sb.AppendLine("Undesired bundles:");
            foreach (var bundle in undesired) {
                sb.AppendLine($"  - {bundle}");
            }
            Debug.LogError(sb.ToString());

            ThrowHelper.ThrowUndesiredException("AssetBundle build result error, see the console output!");
        }
    }
}