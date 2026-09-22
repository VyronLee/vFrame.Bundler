// ------------------------------------------------------------
//         File: SimulationMainAssetAnalyzerBase.cs
//        Brief: Marker base class identifying simulation-mode main-asset analyzers.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:07:38
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Marker base class for simulation-pipeline main-asset analyzers; derives the shared
    ///     analysis services from <see cref="MainAssetAnalyzerBase"/> without adding behavior.
    /// </summary>
    internal abstract class SimulationMainAssetAnalyzerBase : MainAssetAnalyzerBase
    {

    }
}