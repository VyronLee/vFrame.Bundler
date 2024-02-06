// ------------------------------------------------------------
//         File: MainAssetAnalyzerBase.cs
//        Brief: MainAssetAnalyzerBase.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:51
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using vFrame.Bundler.Helper;

namespace vFrame.Bundler.Task.Formal.MainAssetAnalyzers
{
    internal abstract class FormalMainAssetAnalyzerBase : MainAssetAnalyzerBase
    {
        protected bool TryBuiltinAnalyzer(BuildContext context, string path) {
            var ret = false;
            ret |= !AssetHelper.IsBuildableAssets(path);
            ret |= TryAddIfIsShader(context, path);
            ret |= TryAddIfIsScene(context, path);
            return ret;
        }

        private bool TryAddIfIsShader(BuildContext context, string path) {
            if (!AssetHelper.IsShader(path)) {
                return false;
            }
            if (!context.BuildSettings.SeparateShaderBundle) {
                return false;
            }

            var bundlePath = context.BuildSharedShaderBundlePath();
            var assetInfo = new MainAssetInfo {
                AssetPath = path,
                BundlePath = bundlePath
            };
            SafeAddMainAssetInfo(context, assetInfo);
            return true;
        }

        private bool TryAddIfIsScene(BuildContext context, string path) {
            if (!AssetHelper.IsScene(path)) {
                return false;
            }

            var assetInfo = new MainAssetInfo {
                AssetPath = path,
                BundlePath = context.BuildSceneBundlePath(path)
            };
            SafeAddMainAssetInfo(context, assetInfo);
            return true;
        }
    }
}