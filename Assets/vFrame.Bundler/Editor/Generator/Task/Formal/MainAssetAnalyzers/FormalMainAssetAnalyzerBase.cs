// ------------------------------------------------------------
//         File: FormalMainAssetAnalyzerBase.cs
//        Brief: Base for formal main-asset analyzers: builtin handling of non-buildable,
//               separated-shader and scene assets before rule-specific packing.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:19:30
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Base for formal main-asset analyzers: applies builtin handling of non-buildable, separated-shader
    ///     and scene assets before the rule-specific packing logic runs.
    /// </summary>
    internal abstract class FormalMainAssetAnalyzerBase : MainAssetAnalyzerBase
    {
        /// <summary>
        ///     Runs the builtin analyzers on the given asset path.
        /// </summary>
        /// <param name="context">Bundle build context to add main asset infos to.</param>
        /// <param name="path">Asset path to analyze.</param>
        /// <returns><see langword="true"/> if a builtin analyzer claimed the asset; otherwise
        ///     <see langword="false"/>.</returns>
        protected bool TryBuiltinAnalyzer(BuildContext context, string path)
        {
            var ret = false;
            ret |= !AssetHelper.IsBuildableAssets(path);
            ret |= TryAddIfIsShader(context, path);
            ret |= TryAddIfIsScene(context, path);
            return ret;
        }

        /// <summary>
        ///     Assigns a shader asset to the shared shader bundle when
        ///     <see cref="BuildSettings.SeparateShaderBundle"/> is enabled.
        /// </summary>
        /// <param name="context">Bundle build context to add the main asset info to.</param>
        /// <param name="path">Asset path to test.</param>
        /// <returns><see langword="true"/> if the asset is a shader and was added; otherwise
        ///     <see langword="false"/>.</returns>
        private bool TryAddIfIsShader(BuildContext context, string path)
        {
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

        /// <summary>
        ///     Assigns a scene asset to the bundle derived for it.
        /// </summary>
        /// <param name="context">Bundle build context to add the main asset info to.</param>
        /// <param name="path">Asset path to test.</param>
        /// <returns><see langword="true"/> if the asset is a scene and was added; otherwise
        ///     <see langword="false"/>.</returns>
        private bool TryAddIfIsScene(BuildContext context, string path)
        {
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