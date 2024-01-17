// ------------------------------------------------------------
//         File: IPipeline.cs
//        Brief: IPipeline.cs
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