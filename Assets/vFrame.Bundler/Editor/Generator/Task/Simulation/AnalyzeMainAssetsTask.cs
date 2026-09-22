// ------------------------------------------------------------
//         File: AnalyzeMainAssetsTask.cs
//        Brief: Simulation build step 1: applies each MainRules entry by dispatching to the PackType-matched
//               simulation main-asset analyzer to collect directly-loaded main assets.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:22:15
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Simulation build step 1. Applies each main-asset rule (<see cref="BundleBuildRules.MainRules" />) by
    ///     dispatching to the PackType-matched simulation main-asset analyzer to collect directly-loaded main assets.
    /// </summary>
    internal class SimulationAnalyzeMainAssetsTask : BuildTaskBase
    {
        /// <inheritdoc cref="BuildTaskBase.Run" />
        /// <exception cref="BundleArgumentException">
        ///     Thrown if <paramref name="context" />.BuildRules or its MainRules list is null, or a rule's PackType is
        ///     null or empty.
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
                MainAssetAnalyzerBase analyzer = null;
                var packType = Enum.Parse(typeof(PackType), rule.PackType);
                switch (packType) {
                    case PackType.PackBySingleFile:
                        analyzer = new SimulationAnalyzePackBySingleFileRule();
                        break;
                    case PackType.PackByAllFiles:
                        analyzer = new SimulationAnalyzePackByAllFilesRule();
                        break;
                    case PackType.PackByTopDirectory:
                        analyzer = new SimulationAnalyzePackByTopDirectoryRule();
                        break;
                    case PackType.PackByAllDirectories:
                        analyzer = new SimulationAnalyzePackByAllDirectoriesRule();
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