// ------------------------------------------------------------
//         File: AnalyzeMainAssetsTask.cs
//        Brief: AnalyzeMainAssetsTask.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:40
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using vFrame.Bundler.Helper;
using vFrame.Bundler.Task.Simulation.MainAssetAnalyzers;

namespace vFrame.Bundler.Task.Simulation
{
    internal class AnalyzeMainAssetsTask : BuildTaskBase
    {
        public override void Run(BuildContext context) {
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