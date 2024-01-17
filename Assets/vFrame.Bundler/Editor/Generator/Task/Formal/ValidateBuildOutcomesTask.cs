// ------------------------------------------------------------
//         File: ValidateBuildOutComesTask.cs
//        Brief: ValidateBuildOutComesTask.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 10:46
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Linq;
using System.Text;
using UnityEngine;
using vFrame.Bundler.Helper;

namespace vFrame.Bundler.Task.Formal
{
    internal class ValidateBuildOutcomesTask : BuildTaskBase
    {
        public override void Run(BuildContext context) {
            VerifyBundleQuantities(context);
        }

        private void VerifyBundleQuantities(BuildContext context) {
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