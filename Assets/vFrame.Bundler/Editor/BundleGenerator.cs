// ------------------------------------------------------------
//         File: BundleGenerator.cs
//        Brief: BundleGenerator.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-26 22:10
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using vFrame.Bundler.Editor.Pipeline;

namespace vFrame.Bundler.Editor
{
    public static class BundleGenerator
    {
        public static IPipeline BuiltinPipeline => new BuiltinPipeline();

        public static IPipeline ScriptalbePipeline => new ScriptablePipeline();
    }
}