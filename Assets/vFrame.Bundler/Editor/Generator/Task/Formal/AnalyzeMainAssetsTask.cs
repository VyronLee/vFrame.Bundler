// ------------------------------------------------------------
//         File: AnalyzeMainAssetsTask.cs
//        Brief: Formal build step 1: applies each MainRules entry by dispatching to the PackType-matched
//               main-asset analyzer to collect directly-loaded assets.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:19:42
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Formal build step 1. Applies each main-asset rule (<see cref="BundleBuildRules.MainRules" />) by
    ///     dispatching to the PackType-matched main-asset analyzer to collect directly-loaded assets.
    /// </summary>
    internal class AnalyzeMainAssetsTask : BuildTaskBase
    {
        /// <inheritdoc cref="BuildTaskBase.Run" />
        /// <exception cref="BundleArgumentException">
        ///     Thrown if <paramref name="context" />.BuildRules is null, the MainRules list is null or empty, or a
        ///     rule's PackType is null or empty.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown if a rule's PackType is not a valid <see cref="PackType" /> name.</exception>
        /// <exception cref="BundleUnsupportedEnumException">Thrown if a rule's PackType is not a supported value.</exception>
        public override void Run(BuildContext context)
        {
            ThrowHelper.ThrowIfNull(context.BuildRules,
                ThrowHelper.Variables(nameof(context), nameof(context.BuildRules)));
            ThrowHelper.ThrowIfNull(context.BuildRules.MainRules,
                ThrowHelper.Variables(nameof(context), nameof(context.BuildRules), nameof(context.BuildRules.MainRules)));

            var rules = context.BuildRules.MainRules;
            foreach (var rule in rules) {
                ThrowHelper.ThrowIfNullOrEmpty(rule.PackType,
                    ThrowHelper.Variables(nameof(rule), nameof(rule.PackType)));
                FormalMainAssetAnalyzerBase analyzer = null;
                var packType = Enum.Parse(typeof(PackType), rule.PackType);
                switch (packType) {
                    case PackType.PackBySingleFile:
                        analyzer = new AnalyzePackBySingleFileRule();
                        break;
                    case PackType.PackByAllFiles:
                        analyzer = new AnalyzePackByAllFilesRule();
                        break;
                    case PackType.PackByTopDirectory:
                        analyzer = new AnalyzePackByTopDirectoryRule();
                        break;
                    case PackType.PackByAllDirectories:
                        analyzer = new AnalyzePackByAllDirectoriesRule();
                        break;
                    default:
                        ThrowHelper.ThrowUnsupportedEnum(packType);
                        break;
                }
                analyzer?.Run(context, rule);
            }
        }
    }
}