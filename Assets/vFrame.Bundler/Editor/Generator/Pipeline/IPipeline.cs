// ------------------------------------------------------------
//         File: IPipeline.cs
//        Brief: Contract for build pipelines: run a bundle build with the given rules and settings.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-26 22:23
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler.Pipeline
{
    public interface IPipeline
    {
        void Build(BundleBuildRules buildRules, BundleBuildSettings buildSettings);
    }
}